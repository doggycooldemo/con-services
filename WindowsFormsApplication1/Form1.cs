﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSS.TRex.Rendering.Implementations.Framework.GridFabric.Responses;
using VSS.TRex.TAGFiles.Classes.Queues;
using VSS.TRex.TAGFiles.GridFabric.Arguments;
using VSS.TRex.TAGFiles.GridFabric.Requests;
using VSS.TRex.DesignProfiling;
using VSS.TRex.Analytics.Operations;
using VSS.TRex;
using VSS.TRex.Analytics.GridFabric.Arguments;
using VSS.TRex.Analytics.Models;
using VSS.TRex.Designs;
using VSS.TRex.Designs.Storage;
using VSS.TRex.Executors;
using VSS.TRex.Filters;
using VSS.TRex.Geometry;
using VSS.TRex.GridFabric.Affinity;
using VSS.TRex.GridFabric.Caches;
using VSS.TRex.GridFabric.Events;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Queues;
using VSS.TRex.Machines;
using VSS.TRex.Rendering.GridFabric.Arguments;
using VSS.TRex.Rendering.Servers.Client;
using VSS.TRex.Servers;
using VSS.TRex.Servers.Client;
using VSS.TRex.Services.Designs;
using VSS.TRex.Services.Surfaces;
using VSS.TRex.SiteModels;
using VSS.TRex.Storage;
using VSS.TRex.Surfaces;
using VSS.TRex.Types;
using VSS.TRex.Volumes;
using VSS.TRex.Volumes.GridFabric.Arguments;
using VSS.TRex.Volumes.GridFabric.Responses;

namespace VSS.Raptor.IgnitePOC.TestApp
{
    public partial class Form1 : Form
    {
        BoundingWorldExtent3D extents = BoundingWorldExtent3D.Inverted();

        //GenericApplicationServiceServer genericApplicationServiceServer = new GenericApplicationServiceServer();
        TileRenderingServer tileRenderServer;
        SimpleVolumesServer simpleVolumesServer;
        MutableClientServer mutableClient;

        SiteModelAttributesChangedEventListener SiteModelAttrubutesChanged;

        /// <summary>
        /// Convert the Project ID in the text box into a number. It if is invalid return project ID 2 as a default
        /// </summary>
        /// <returns></returns>
        private Guid ID()
        {
            try
            {
                return Guid.Parse(editProjectID.Text);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private System.Drawing.Bitmap PerformRender(DisplayMode displayMode, int width, int height, bool returnEarliestFilteredCellPass, BoundingWorldExtent3D extents)
        {
            // Get the relevant SiteModel. Use the generic application service server to instantiate the Ignite instance
            // SiteModel siteModel = GenericApplicationServiceServer.PerformAction(() => SiteModels.Instance().GetSiteModel(ID, false));
            SiteModel siteModel = SiteModels.Instance().GetSiteModel(ID(), false);

            if (siteModel == null)
            {
                MessageBox.Show($"Site model {ID()} is unavailable");
                return null;
            }

            try
            {
                CellPassAttributeFilter AttributeFilter = new CellPassAttributeFilter(/*siteModel*/)
                {
                    ReturnEarliestFilteredCellPass = returnEarliestFilteredCellPass,
                    HasElevationTypeFilter = true,
                    ElevationType = returnEarliestFilteredCellPass ? ElevationType.First : ElevationType.Last,
                    SurveyedSurfaceExclusionList = GetSurveyedSurfaceExclusionList(siteModel)
                };

                CellSpatialFilter SpatialFilter = new CellSpatialFilter()
                {
                    CoordsAreGrid = true,
                    IsSpatial = true,
                    Fence = new Fence(extents)
                };

                TileRenderResponse_Framework response = tileRenderServer.RenderTile(new TileRenderRequestArgument
                (ID(),
                 displayMode,
                 extents,
                 true, // CoordsAreGrid
                 (ushort)width, // PixelsX
                 (ushort)height, // PixelsY
                 new CombinedFilter(AttributeFilter, SpatialFilter), // Filter1
                 null, // filter 2
                 (cmbDesigns.Items.Count == 0) ? long.MinValue : (cmbDesigns.SelectedValue as Design).ID// DesignDescriptor
                )) as TileRenderResponse_Framework;

                return response?.TileBitmap;
            }
            catch (Exception E)
            {
                MessageBox.Show($"Exception: {E}");
                return null;
            }
        }

        private BoundingWorldExtent3D GetZoomAllExtents()
        {
            SiteModel siteModel = SiteModels.Instance().GetSiteModel(ID(), false);

            if (siteModel != null)
            {
                long[] SurveyedSurfaceExclusionList = (siteModel.SurveyedSurfaces == null || chkIncludeSurveyedSurfaces.Checked) ? new long[0] : siteModel.SurveyedSurfaces.Select(x => x.ID).ToArray();

                return ProjectExtents.ProductionDataAndSurveyedSurfaces(ID(), SurveyedSurfaceExclusionList);
            }
            else
            {
                return BoundingWorldExtent3D.Null();
            }
        }

        public Form1()
        {
            InitializeComponent();

            // Set the display modes in the combo box
            displayMode.Items.AddRange(Enum.GetNames(typeof(DisplayMode)));
            displayMode.SelectedIndex = (int)DisplayMode.Height;

            tileRenderServer = TileRenderingServer.NewInstance(new[] { ApplicationServiceServer.DEFAULT_ROLE_CLIENT, ServerRoles.TILE_RENDERING_NODE });
            simpleVolumesServer = SimpleVolumesServer.NewInstance(new [] { ApplicationServiceServer.DEFAULT_ROLE_CLIENT });
            mutableClient = new MutableClientServer("TestApplication");

            // Instantiate a site model changed listener to catch changes to site model attributes
            SiteModelAttrubutesChanged = new SiteModelAttributesChangedEventListener(TRexGrids.ImmutableGridName());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void fitExtentsToView(int width, int height)
        {
            double Aspect = (1.0 * height) / (1.0 * width);

            if ((extents.SizeX / extents.SizeY) > Aspect)
            {
                extents = new BoundingWorldExtent3D(extents.CenterX - (extents.SizeY / Aspect) / 2,
                                                    extents.CenterY - (extents.SizeY / 2),
                                                    extents.CenterX + (extents.SizeY / Aspect) / 2,
                                                    extents.CenterY + (extents.SizeY / 2),
                                                    extents.MinZ, extents.MaxZ);
            }
            else
            {
                extents = new BoundingWorldExtent3D(extents.CenterX - (extents.SizeX / 2),
                                                    extents.CenterY - (extents.SizeX * Aspect) / 2,
                                                    extents.CenterX + (extents.SizeX / 2),
                                                    extents.CenterY + (extents.SizeX * Aspect) / 2,
                                                    extents.MinZ, extents.MaxZ);
            }
            /*

                                    // Modify extents to match the shape of the panel it is being displayed in
                                    if ((extents.SizeX / extents.SizeY) < (width / height))
                                    {
                                        double pixelSize = extents.SizeX / width;
                            extents = new BoundingWorldExtent3D(extents.CenterX - (width / 2) * pixelSize,
                                                                extents.CenterY - (height / 2) * pixelSize,
                                                                extents.CenterX + (width / 2) * pixelSize,
                                                                extents.CenterY + (height / 2) * pixelSize);//,
            //                                                    extents.MinZ, extents.MaxZ);
                                    }
                                    else
                                    {
                                        double pixelSize = extents.SizeY / height;
                            extents = new BoundingWorldExtent3D(extents.CenterX - (width / 2) * pixelSize,
                                                                extents.CenterY - (height / 2) * pixelSize,
                                                                extents.CenterX + (width / 2) * pixelSize,
                                                                extents.CenterY + (height / 2) * pixelSize); //,                                                                
            //                                                    extents.MinZ, extents.MaxZ);
        }            
            */
        }

        private void DoRender()
        {
            fitExtentsToView(pictureBox1.Width, pictureBox1.Height);

            System.Drawing.Bitmap bmp = PerformRender((DisplayMode)displayMode.SelectedIndex, pictureBox1.Width, pictureBox1.Height, chkSelectEarliestPass.Checked, extents);

            if (bmp != null)
            {
                bmp.Save(@"C:\temp\renderedtile.bmp", ImageFormat.Bmp);
                pictureBox1.Image = bmp;
                pictureBox1.Show();
            }
        }

        private void DoUpdateLabels()
        {
            lblViewHeight.Text = $"View height: {extents.SizeY:F3}m";
            lblViewWidth.Text = $"View width: {extents.SizeX:F3}m";
            lblCellsPerPixel.Text = $"Cells Per Pixel (X): {(extents.SizeX / pictureBox1.Width) / 0.34:F3}";
        }

        private void DoUpdateDesignsAndSurveyedSurfaces()
        {
            Designs designs = DesignsService.Instance().List(ID());

            if (designs != null)
            {
                cmbDesigns.DropDownStyle = ComboBoxStyle.DropDownList;

                //cmbDesigns.Items.Clear();

                cmbDesigns.DisplayMember = "Text";
                cmbDesigns.ValueMember = "Value";
                cmbDesigns.DataSource = designs.Select(x => new { Text = x.DesignDescriptor.FullPath, Value = x }).ToArray();
            }

            SurveyedSurfaceService surveyedSurfacesService = new SurveyedSurfaceService(StorageMutability.Immutable);
            surveyedSurfacesService.Init(null);
            SurveyedSurfaces surveyedSurfaces = surveyedSurfacesService.List(ID());

            if (surveyedSurfaces != null)
            {
                cmbDesigns.DropDownStyle = ComboBoxStyle.DropDownList;

                //cmbSurveyedSurfaces.Items.Clear();

                cmbSurveyedSurfaces.DisplayMember = "Text";
                cmbSurveyedSurfaces.ValueMember = "Value";
                cmbSurveyedSurfaces.DataSource = surveyedSurfaces.Select(x => new { Text = x.DesignDescriptor.FullPath, Value = x }).ToArray();
            }
        }

        private void DoScreenUpdate()
        {
            DoUpdateLabels();
            DoRender();
        }

        private void ViewPortChange(Action viewPortAction)
        {
          Cursor.Current = Cursors.WaitCursor;
          viewPortAction();
          DoScreenUpdate();
          Cursor.Current = Cursors.Default;
    }

        private void ZoomAll_Click(object sender, EventArgs e)
        {
            // Get the project extent so we know where to render
            ViewPortChange(() => extents = GetZoomAllExtents());
            // GetAdjustedDataModelSpatialExtents
        }

        private void btmZoomIn_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.ScalePlan(0.8));
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.ScalePlan(1.25));
        }

        private double translationIncrement() => 0.2 * (extents.SizeX > extents.SizeY ? extents.SizeY : extents.SizeX);

        private void btnTranslateNorth_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.Offset(0, translationIncrement()));
        }

        private void bntTranslateWest_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.Offset(-translationIncrement(), 0));
        }

        private void bntTranslateEast_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.Offset(translationIncrement(), 0));
        }

        private void bntTranslateSouth_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => extents.Offset(0, -translationIncrement()));
        }

        private void btnRedraw_Click(object sender, EventArgs e)
        {
            ViewPortChange(() => { });
        }

        private void editProjectID_TextChanged(object sender, EventArgs e)
        {
            // Pull the sitemodel extents using the ProjectExtents executor which will use the Ignite instance created by the generic application service server above
            if (ID() != Guid.Empty)
            {
                extents = GetZoomAllExtents();
                DoUpdateLabels();
                DoUpdateDesignsAndSurveyedSurfaces();
            }
        }

        private void btnMultiThreadTest_Click(object sender, EventArgs e)
        {
            int nImages = Convert.ToInt32(edtNumImages.Text);
            int nRuns = Convert.ToInt32(edtNumRuns.Text);

            DisplayMode displayMode = (DisplayMode)this.displayMode.SelectedIndex;
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            bool selectEarliestPass = chkSelectEarliestPass.Checked;

            //            Bitmap[] bitmaps = new Bitmap[nImages];

            fitExtentsToView(width, height);

            // Construct an array of identical bitmaps that are displayed on the form to see how well it multi-threads the requests

            StringBuilder results = new StringBuilder();

            for (int i = 0; i < nRuns; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // Parallel
                Parallel.For(0, nImages, x =>
                {
                    using (System.Drawing.Bitmap b = PerformRender(displayMode, width, height, selectEarliestPass, extents))
                    {
                    }
                });
                // Linear
                //for (int count = 0; count < nImages; count++)
                //{  
                //    using (Bitmap b = PerformRender(displayMode, width, height, selectEarliestPass, extents))
                //    {
                //    }
                //};


                sw.Stop();

                results.Append($"Run {i}: Images:{nImages}, Time:{sw.Elapsed}\n");
            }

            MessageBox.Show($"Results:\n{results.ToString()}");
            //MessageBox.Show(String.Format("Images:{0}, Time:{1}", nImages, sw.Elapsed));
        }

        private void writeCacheMetrics(StreamWriter writer, ICacheMetrics metrics)
        {
            writer.WriteLine($"Number of items in cache: {metrics.Size}");
        }

        private void writeKeys(string title, StreamWriter writer, ICache<NonSpatialAffinityKey, byte[]> cache)
        {
            int count = 0;

            writer.WriteLine(title);
            writer.WriteLine("###############");
            writer.WriteLine();

            if (cache == null)
            {
                return;
            }

            //            writeCacheMetrics(writer, cache.GetMetrics());

            var scanQuery = new ScanQuery<NonSpatialAffinityKey, byte[]>();
            IQueryCursor<ICacheEntry<NonSpatialAffinityKey, byte[]>> queryCursor = cache.Query(scanQuery);
            scanQuery.PageSize = 1; // Restrict the number of keys requested in each page to reduce memory consumption

            foreach (ICacheEntry<NonSpatialAffinityKey, byte[]> cacheEntry in queryCursor)
            {
                writer.WriteLine($"{count++}:{cacheEntry.Key}, size = {cacheEntry.Value.Length}");
                //                writeCacheMetrics(writer, cache.GetMetrics());
            }

            writer.WriteLine();
        }

        private void writeTAGFileBufferQueueKeys(string title, StreamWriter writer, ICache<TAGFileBufferQueueKey, TAGFileBufferQueueItem> cache)
        {
            int count = 0;

            writer.WriteLine(title);
            writer.WriteLine("#####################");
            writer.WriteLine();

            if (cache == null)
            {
                return;
            }

            var scanQuery = new ScanQuery<TAGFileBufferQueueKey, TAGFileBufferQueueItem>();
            IQueryCursor<ICacheEntry<TAGFileBufferQueueKey, TAGFileBufferQueueItem>> queryCursor = cache.Query(scanQuery);
            scanQuery.PageSize = 1; // Restrict the number of keys requested in each page to reduce memory consumption

            foreach (ICacheEntry<TAGFileBufferQueueKey, TAGFileBufferQueueItem> cacheEntry in queryCursor)
            {
                writer.WriteLine($"{count++}:{cacheEntry.Key}, size = {cacheEntry.Value.Content.Length}");
            }

            writer.WriteLine();
        }

        private void WriteKeysSpatial(string title, StreamWriter writer, ICache<SubGridSpatialAffinityKey, byte[]> cache)
        {
            int count = 0;

            writer.WriteLine(title);
            writer.WriteLine("###############");
            writer.WriteLine();

            if (cache == null)
            {
                return;
            }

            // writeCacheMetrics(writer, cache.GetMetrics());

            var scanQuery = new ScanQuery<SubGridSpatialAffinityKey, byte[]>
            {
                PageSize = 1 // Restrict the number of keys requested in each page to reduce memory consumption
            };

            IQueryCursor<ICacheEntry<SubGridSpatialAffinityKey, byte[]>> queryCursor = cache.Query(scanQuery);

            foreach (ICacheEntry<SubGridSpatialAffinityKey, byte[]> cacheEntry in queryCursor)
            {
                writer.WriteLine($"{count++}:{cacheEntry.Key}, size = {cacheEntry.Value.Length}");
                // writeCacheMetrics(writer, cache.GetMetrics());
            }

            writer.WriteLine();
        }

        private void retriveAllItems(string title, StreamWriter writer, ICache<SubGridSpatialAffinityKey, byte[]> cache)
        {
            var scanQuery = new ScanQuery<SubGridSpatialAffinityKey, byte[]>
            {
                PageSize = 1 // Restrict the number of keys requested in each page to reduce memory consumption
            };

            IQueryCursor<ICacheEntry<SubGridSpatialAffinityKey, byte[]>> queryCursor = cache.Query(scanQuery);

            List<SubGridSpatialAffinityKey> items = new List<SubGridSpatialAffinityKey>();

            foreach (ICacheEntry<SubGridSpatialAffinityKey, byte[]> cacheEntry in queryCursor)
            {
                items.Add(cacheEntry.Key);
            }

            scanQuery = null;

            foreach (SubGridSpatialAffinityKey item in items)
            {
                byte[] Entry = cache.Get(item);
            }

            scanQuery = null;
        }

        private void dumpKeysToFile(StorageMutability mutability, string fileName)
        {
            try
            {
                IIgnite ignite = RaptorGridFactory.Grid(TRexGrids.GridName(mutability));

                if (ignite == null)
                {
                    MessageBox.Show($"No ignite reference for {TRexGrids.GridName(mutability)} grid");
                    return;
                }

                using (var outFile = new FileStream(fileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(outFile))
                    {
                        if (mutability == StorageMutability.Immutable)
                        {
                            try
                            {
                                writeKeys(TRexCaches.ImmutableNonSpatialCacheName(), writer, ignite.GetCache<NonSpatialAffinityKey, byte[]>(TRexCaches.ImmutableNonSpatialCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                            try
                            {
                                writeKeys(TRexCaches.DesignTopologyExistenceMapsCacheName(), writer, ignite.GetCache<NonSpatialAffinityKey, byte[]>(TRexCaches.DesignTopologyExistenceMapsCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                            try
                            {
                                WriteKeysSpatial(TRexCaches.ImmutableSpatialCacheName(), writer, ignite.GetCache<SubGridSpatialAffinityKey, byte[]>(TRexCaches.ImmutableSpatialCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                        }
                        if (mutability == StorageMutability.Mutable)
                        {
                            try
                            {
                                writeKeys(TRexCaches.MutableNonSpatialCacheName(), writer, ignite.GetCache<NonSpatialAffinityKey, byte[]>(TRexCaches.MutableNonSpatialCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                            try
                            {
                                WriteKeysSpatial(TRexCaches.MutableSpatialCacheName(), writer, ignite.GetCache<SubGridSpatialAffinityKey, byte[]>(TRexCaches.MutableSpatialCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                            try
                            {
                                writeTAGFileBufferQueueKeys(TRexCaches.TAGFileBufferQueueCacheName(), writer, ignite.GetCache<TAGFileBufferQueueKey, TAGFileBufferQueueItem>(TRexCaches.TAGFileBufferQueueCacheName()));
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show($"Exception occurred: {E}");
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtain all the keys and write them into files
            dumpKeysToFile(StorageMutability.Mutable, @"C:\Temp\AllRaptorIgniteCacheKeys = mutable.txt");
            dumpKeysToFile(StorageMutability.Immutable, @"C:\Temp\AllRaptorIgniteCacheKeys = immutable.txt");
        }

        private void chkSelectEarliestPass_CheckedChanged(object sender, EventArgs e)
        {
            DoScreenUpdate();
        }

        private void chkIncludeSurveyedSurfaces_CheckedChanged(object sender, EventArgs e)
        {
            DoScreenUpdate();
        }

        /// <summary>
        /// Calculate statistics across a single cache in the grid
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        private string CalculateCacheStatistics(string title, ICache<object, byte[]> cache)
        {

            if (cache == null)
            {
                return $"Cache {title} is null";
            }

            var scanQuery = new ScanQuery<object, byte[]>
            {
                PageSize = 1 // Restrict the number of keys requested in each page to reduce memory consumption
            };

            long count = 0;
            long sumBytes = 0;
            long smallestBytes = long.MaxValue;
            long largestBytes = long.MinValue;

            IQueryCursor<ICacheEntry<object, byte[]>> queryCursor = cache.Query(scanQuery);

            foreach (ICacheEntry<object, byte[]> cacheEntry in queryCursor)
            {
                count++;
                sumBytes += cacheEntry.Value.Length;
                if (smallestBytes > cacheEntry.Value.Length) smallestBytes = cacheEntry.Value.Length;
                if (largestBytes < cacheEntry.Value.Length) largestBytes = cacheEntry.Value.Length;
            }

            if (count == 0) return $"No elements in cache {title}";

            return $"Cache {title}: {count} entries totalling {sumBytes} bytes. Average: {sumBytes / count} bytes, smallest: {smallestBytes} bytes, largest: {largestBytes} bytes";
        }

        /// <summary>
        /// Calculate statistics on the numbers and sizes of elements in the major Raptor caches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This may take some time...", "Confirmation", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                IIgnite ignite = RaptorGridFactory.Grid(TRexGrids.MutableGridName());

                if (ignite != null)
                {
                    string result = CalculateCacheStatistics(TRexCaches.MutableNonSpatialCacheName(), ignite.GetCache<object, byte[]>(TRexCaches.MutableNonSpatialCacheName())) + "\n" +
                                    CalculateCacheStatistics(TRexCaches.MutableSpatialCacheName(), ignite.GetCache<object, byte[]>(TRexCaches.MutableSpatialCacheName()));
                    MessageBox.Show(result, "Mutable Statistics");
                }
                else
                {
                    MessageBox.Show("No Ignite referece for mutable Statistics");
                }

                ignite = RaptorGridFactory.Grid(TRexGrids.ImmutableGridName());
                if (ignite != null)
                {
                    string result = CalculateCacheStatistics(TRexCaches.ImmutableNonSpatialCacheName(), ignite.GetCache<object, byte[]>(TRexCaches.ImmutableNonSpatialCacheName())) + "\n" +
                                    CalculateCacheStatistics(TRexCaches.ImmutableSpatialCacheName(), ignite.GetCache<object, byte[]>(TRexCaches.ImmutableSpatialCacheName()));
                    MessageBox.Show(result, "Immutable Statistics");
                }
                else
                {
                    MessageBox.Show("No Ignite referece for immutable Statistics");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString(), "An error occurred");
            }
        }

        /// <summary>
        /// Perform a simple volumes calc based on earliest to latest filters of the viewable screen area.
        /// </summary>
        /// <returns></returns>
        private SimpleVolumesResponse PerformVolume(bool useScreenExtents)
        {
            // Get the relevant SiteModel. Use the generic application service server to instantiate the Ignite instance
            // SiteModel siteModel = RaptorGenericApplicationServiceServer.PerformAction(() => SiteModels.Instance().GetSiteModel(ID, false));
            SiteModel siteModel = SiteModels.Instance().GetSiteModel(ID(), false);

            try
            {
                // Create the two filters
                CombinedFilter FromFilter = new CombinedFilter()
                {
                    AttributeFilter = new CellPassAttributeFilter(/*siteModel*/)
                    {
                        ReturnEarliestFilteredCellPass = true,
                        HasElevationTypeFilter = true,
                        ElevationType = ElevationType.First,
                        SurveyedSurfaceExclusionList = GetSurveyedSurfaceExclusionList(siteModel),
                    },

                    SpatialFilter = !useScreenExtents 
                    ? new CellSpatialFilter()
                    : new CellSpatialFilter
                    {
                        CoordsAreGrid = true,
                        IsSpatial = true,
                        Fence = new Fence(extents)
                    }
                };

                CombinedFilter ToFilter = new CombinedFilter()
                {
                    AttributeFilter = new CellPassAttributeFilter(/*siteModel*/)
                    {
                        ReturnEarliestFilteredCellPass = false,
                        HasElevationTypeFilter = true,
                        ElevationType = ElevationType.Last,
                        SurveyedSurfaceExclusionList = GetSurveyedSurfaceExclusionList(siteModel),
                    },

                    SpatialFilter = FromFilter.SpatialFilter
                };

                return simpleVolumesServer.ComputeSimpleVolues(new SimpleVolumesRequestArgument()
                {
                    SiteModelID = ID(),
                    BaseFilter = FromFilter,
                    TopFilter = ToFilter,
                    VolumeType = VolumeComputationType.Between2Filters
                });
            }
            catch (Exception E)
            {
                MessageBox.Show($@"Exception: {E}");
                return null;
            }
        }

        /// <summary>
        /// Determine the list of survyeed surfaces that shoudl be excluded depeneding on the
        /// sitemodel and state of relevant UI components
        /// </summary>
        /// <param name="siteModel"></param>
        /// <returns></returns>
        private long[] GetSurveyedSurfaceExclusionList(SiteModel siteModel) => (siteModel.SurveyedSurfaces == null || chkIncludeSurveyedSurfaces.Checked) ? new long[0] : siteModel.SurveyedSurfaces.Select(x => x.ID).ToArray();

        private void btnCalculateVolumes_Click(object sender, EventArgs e)
        {
      // Calculate a simple volume based on a filter to filter, earliest to latest context, for the visible extents on the screen
            Cursor.Current = Cursors.WaitCursor;
            SimpleVolumesResponse volume = PerformVolume(true);

            if (volume == null)
            {
                MessageBox.Show("Volume retuned no response");
                return;
            }
          Cursor.Current = Cursors.Default;

          MessageBox.Show($"Simple Volume Response [Screen Extents]:\n{volume}");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Make a test queue object and see how it goes
            TestQueueHolder queue = new TestQueueHolder();

            IEnumerable<TestQueueItem> result = queue.Query(DateTime.Now);

            if (result?.Count() > 0)
            {
                MessageBox.Show($"Items: {result.Aggregate("", (accumulator, item) => accumulator + item.Value + " ")}");
            }
            else
            {
                MessageBox.Show("Query result is null or empty");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Test adding a stream for "<NewID>-ProductionDataModel.XML" to the mutable non-spatial cache 

            IIgnite ignite = Ignition.GetIgnite(TRexGrids.MutableGridName());

            ICache<NonSpatialAffinityKey, byte[]> cache = ignite.GetCache<NonSpatialAffinityKey, byte[]>(TRexCaches.MutableNonSpatialCacheName());

            byte[] bytes = new byte[100];

            NonSpatialAffinityKey cacheKey = new NonSpatialAffinityKey(Guid.NewGuid(), "ProductionDataModel.XML");

            try
            {
                cache.Put(cacheKey, bytes);

                byte[] readBytes = cache.Get(cacheKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Calculate cut fill statistics from the latest elevations to the selected design

            var siteModel = SiteModels.Instance().GetSiteModel(ID(), false);
            var offsets = new [] { 0.5, 0.2, 0.1, 0, -0.1, -0.2, -0.5 };

            Stopwatch sw = new Stopwatch();
            sw.Start();

            CutFillResult result = CutFillOperation.Execute(new CutFillStatisticsArgument()
            {
                DataModelID = siteModel.ID,
                Filter = new CombinedFilter(),
                //{
                //    AttributeFilter = new CellPassAttributeFilter(/*siteModel*/),
                //    SpatialFilter = new CellSpatialFilter()
                //},
                DesignID = (cmbDesigns.Items.Count == 0) ? long.MinValue : (cmbDesigns.SelectedValue as Design).ID,
                Offsets = offsets
            });

            // Show the list of percentages calculated by the request
            MessageBox.Show($"Results (in {sw.Elapsed}) [Cut/Fill:{offsets.Aggregate("", (a, v) => a + $"{ v.ToString("F1")}, ")}]: {(result?.Percents == null ? "No Result" : result.Percents?.Aggregate("", (a, v) => a + $"{v.ToString("F1")}% "))}");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Tests the time taken to perform 10,000 full TTM patch requests for a test design, both locally, and by accessing a 
            // remote DesignElevation service

            TTMDesign design = new TTMDesign(SubGridTree.DefaultCellSize);
            design.LoadFromFile(@"C:\Temp\Bug36372.ttm");
            const int numPatches = 10000;

            float[,] Patch = new float[SubGridTree.SubGridTreeDimension, SubGridTree.SubGridTreeDimension];

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < numPatches; i++)
            {
                bool result = design.InterpolateHeights(Patch, 247500.0, 193350.0, SubGridTree.DefaultCellSize, 0);
            }

            MessageBox.Show($"{numPatches} patches requested in {sw.Elapsed}, {(numPatches * 1024.0) / (sw.ElapsedMilliseconds / 1000.0)} per second");

            Design ttmDesign = new Design(-1, new DesignDescriptor(-1, "", "", @"C:\Temp\", "Bug36372.ttm", 0.0), extents);
            sw.Reset();
            sw.Start();

            for (int i = 0; i < numPatches; i++)
            {
                bool result = design.InterpolateHeights(Patch, 247500.0, 193350.0, SubGridTree.DefaultCellSize, 0);
            }

            MessageBox.Show($"{numPatches} patches requested in {sw.Elapsed}, {(numPatches * 1024.0) / (sw.ElapsedMilliseconds / 1000.0)} per second");
        }

    private void btnRedraw_Click_1(object sender, EventArgs e)
    {

    }

      private void button7_Click(object sender, EventArgs e)
      {

        Machine machine = new Machine(null, "TestName", "TestHardwareID", 0, 0, Guid.NewGuid(), 0, false);

        SubmitTAGFileRequest request = new SubmitTAGFileRequest();
        SubmitTAGFileRequestArgument arg = null;
        string fileName =
            "J:\\PP\\Construction\\Office software\\SiteVision Office\\Test Files\\VisionLink Data\\Southern Motorway\\TAYLORS COMP\\IgniteTestData\\0201J004SV--TAYLORS COMP--110504215856.tag";

        string tccOrgID = new Guid().ToString(); 

        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
          byte[] bytes = new byte[fs.Length];
          fs.Read(bytes, 0, bytes.Length);

          arg = new SubmitTAGFileRequestArgument()
                {
                    ProjectID = ID(),
                    AssetID = Guid.Parse("1414327a-2b91-41ef-9390-6c1f3ccc73c9"), // machine.ID,
                    TAGFileName = "0201J004SV--TAYLORS COMP--110504215856.tag",
                    TagFileContent = bytes,
                    TCCOrgID = tccOrgID
                };

        }
        request.Execute(arg);
      }

        private void button8_Click(object sender, EventArgs e)
        {
            // Calculate a simple volume based on a filter to filter, earliest to latest context for the entire model
            Cursor.Current = Cursors.WaitCursor;
            SimpleVolumesResponse volume = PerformVolume(false);

            if (volume == null)
            {
                MessageBox.Show("Volume retuned no response");
                return;
            }
            Cursor.Current = Cursors.Default;

            MessageBox.Show($"Simple Volume Response [Model Extents]:\n{volume}");
        }
    }
}
