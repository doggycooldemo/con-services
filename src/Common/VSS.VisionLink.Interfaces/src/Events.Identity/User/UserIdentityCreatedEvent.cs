﻿using System;
using VSS.VisionLink.Interfaces.Events.Identity.Interfaces;

namespace VSS.VisionLink.Interfaces.Events.Identity.User
{
    /// <summary>
    /// UserIdentityCreatedEvent Model
    /// </summary>
    public class UserIdentityCreatedEvent : IUserIdentityEvent
    {
        /// <summary>
        /// Id generated by Identity System during UserIdentity Creation
        /// </summary>
        public string UserUID { get; set; }
        /// <summary>
        /// LoginId of the User
        /// </summary>
        public string LoginId { get; set; }
        /// <summary>
        /// FirstName of the User
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// LastName of the User
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Email address of the User - This Field will hold value as same as the LoginId
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Address of the User - This field is retired, will be published as empty going forward.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// This field holds the AddressLine1 value
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// This field holds the AddressLine2 value
        /// </summary>
        public string AddressLine2 { get; set; }
        /// <summary>
        /// This field holds the City value
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// This field holds the State value
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// This field holds the Zip value
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// This field holds the Country value
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Phone Number of the User
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// This field holds the values Standard,SSO etc.,
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// Represents the date time when the user was migrated/created in TPaaS.
        /// This might be differ from the insert date time in CG database
        /// </summary>
        public DateTime ActionUTC { get; set; }
        /// <summary>
        /// Represents the date time when this message was sent to message queue (kafka)
        /// </summary>
        public DateTime ReceivedUTC { get; set; }
    }
}