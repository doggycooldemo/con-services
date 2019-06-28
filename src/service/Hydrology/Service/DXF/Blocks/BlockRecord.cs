﻿using System;
using VSS.Hydrology.WebApi.DXF.Collections;
using VSS.Hydrology.WebApi.DXF.Objects;
using VSS.Hydrology.WebApi.DXF.Tables;
using VSS.Hydrology.WebApi.DXF.Units;

namespace VSS.Hydrology.WebApi.DXF.Blocks
{
    /// <summary>
    /// Represent the record of a block in the tables section.
    /// </summary>
    public class BlockRecord :
        DxfObject,
        IHasXData
    {
        #region delegates and events

        //public delegate void XDataAddAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

        public event XDataAddAppRegEventHandler XDataAddAppReg;

        protected virtual void OnXDataAddAppRegEvent(ApplicationRegistry item)
        {
            XDataAddAppRegEventHandler ae = this.XDataAddAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        //public delegate void XDataRemoveAppRegEventHandler(EntityObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

        public event XDataRemoveAppRegEventHandler XDataRemoveAppReg;

        protected virtual void OnXDataRemoveAppRegEvent(ApplicationRegistry item)
        {
            XDataRemoveAppRegEventHandler ae = this.XDataRemoveAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        #endregion

        #region private fields

        private string name;
        private Layout layout;
        private static DrawingUnits defaultUnits = DrawingUnits.Unitless;
        private DrawingUnits units;
        private bool allowExploding;
        private bool scaleUniformly;
        private readonly XDataDictionary xData;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>BlockRecord</c> class.
        /// </summary>
        /// <param name="name">Block definition name.</param>
        internal BlockRecord(string name)
            : base(DxfObjectCode.BlockRecord)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            this.name = name;
            this.layout = null;
            this.units = DefaultUnits;
            this.allowExploding = true;
            this.scaleUniformly = false;
            this.xData = new XDataDictionary();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the name of the block record.
        /// </summary>
        /// <remarks>
        /// Block record names are case insensitive.<br />
        /// The block which name starts with "*" are for internal purpose only.
        /// </remarks>
        public string Name
        {
            get { return this.name; }
            internal set { this.name = value; }
        }

        /// <summary>
        /// Gets the associated Layout.
        /// </summary>
        public Layout Layout
        {
            get { return this.layout; }
            internal set { this.layout = value; }
        }

        /// <summary>
        /// Gets or sets the block insertion units.
        /// </summary>
        public DrawingUnits Units
        {
            get { return this.units; }
            set { this.units = value; }
        }

        /// <summary>
        /// Gets or sets the default block units.
        /// </summary>
        /// <remarks>These are the units that all new blocks will use as default.</remarks>
        public static DrawingUnits DefaultUnits
        {
            get { return defaultUnits; }
            set { defaultUnits = value; }
        }

        /// <summary>
        /// Gets or sets if the block can be exploded.
        /// </summary>
        /// <remarks>
        /// This property is only compatible with dxf version AutoCad2007 and upwards.
        /// </remarks>
        public bool AllowExploding
        {
            get { return this.allowExploding; }
            set { this.allowExploding = value; }
        }

        /// <summary>
        /// Gets or sets if the block must be scaled uniformly.
        /// </summary>
        /// <remarks>
        /// This property is only compatible with dxf version AutoCad2007 and upwards.
        /// </remarks>
        public bool ScaleUniformly
        {
            get { return this.scaleUniformly; }
            set { this.scaleUniformly = value; }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new BlockRecords Owner
        {
            get { return (BlockRecords) base.Owner; }
            internal set { base.Owner = value; }
        }

        /// <summary>
        /// Gets if the block record is for internal use only.
        /// </summary>
        /// <remarks>
        /// All blocks which name starts with "*" are for internal use and should not be modified.
        /// </remarks>
        public bool IsForInternalUseOnly
        {
            get { return this.name.StartsWith("*"); }
        }

        /// <summary>
        /// Gets the block record <see cref="XDataDictionary">extended data</see>.
        /// </summary>
        public XDataDictionary XData
        {
            get { return this.xData; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
