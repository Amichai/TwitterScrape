﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("TweetDataModel", "LinkSite", "Website", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(DataStore.Website), "Tweet", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(DataStore.Tweet), true)]

#endregion

namespace DataStore
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class TweetDataEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new TweetDataEntities object using the connection string found in the 'TweetDataEntities' section of the application configuration file.
        /// </summary>
        public TweetDataEntities() : base("name=TweetDataEntities", "TweetDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new TweetDataEntities object.
        /// </summary>
        public TweetDataEntities(string connectionString) : base(connectionString, "TweetDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new TweetDataEntities object.
        /// </summary>
        public TweetDataEntities(EntityConnection connection) : base(connection, "TweetDataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Website> Websites
        {
            get
            {
                if ((_Websites == null))
                {
                    _Websites = base.CreateObjectSet<Website>("Websites");
                }
                return _Websites;
            }
        }
        private ObjectSet<Website> _Websites;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Tweet> Tweets
        {
            get
            {
                if ((_Tweets == null))
                {
                    _Tweets = base.CreateObjectSet<Tweet>("Tweets");
                }
                return _Tweets;
            }
        }
        private ObjectSet<Tweet> _Tweets;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Medium> Media
        {
            get
            {
                if ((_Media == null))
                {
                    _Media = base.CreateObjectSet<Medium>("Media");
                }
                return _Media;
            }
        }
        private ObjectSet<Medium> _Media;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Websites EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToWebsites(Website website)
        {
            base.AddObject("Websites", website);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Tweets EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToTweets(Tweet tweet)
        {
            base.AddObject("Tweets", tweet);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Media EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToMedia(Medium medium)
        {
            base.AddObject("Media", medium);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="TweetDataModel", Name="Medium")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Medium : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Medium object.
        /// </summary>
        /// <param name="url">Initial value of the Url property.</param>
        /// <param name="sourceSite">Initial value of the SourceSite property.</param>
        public static Medium CreateMedium(global::System.String url, global::System.String sourceSite)
        {
            Medium medium = new Medium();
            medium.Url = url;
            medium.SourceSite = sourceSite;
            return medium;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url != value)
                {
                    OnUrlChanging(value);
                    ReportPropertyChanging("Url");
                    _Url = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }
        private global::System.String _Url;
        partial void OnUrlChanging(global::System.String value);
        partial void OnUrlChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                ReportPropertyChanging("Type");
                _Type = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Type");
                OnTypeChanged();
            }
        }
        private global::System.String _Type;
        partial void OnTypeChanging(global::System.String value);
        partial void OnTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String SourceSite
        {
            get
            {
                return _SourceSite;
            }
            set
            {
                OnSourceSiteChanging(value);
                ReportPropertyChanging("SourceSite");
                _SourceSite = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("SourceSite");
                OnSourceSiteChanged();
            }
        }
        private global::System.String _SourceSite;
        partial void OnSourceSiteChanging(global::System.String value);
        partial void OnSourceSiteChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ImageArea
        {
            get
            {
                return _ImageArea;
            }
            set
            {
                OnImageAreaChanging(value);
                ReportPropertyChanging("ImageArea");
                _ImageArea = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ImageArea");
                OnImageAreaChanged();
            }
        }
        private Nullable<global::System.Int32> _ImageArea;
        partial void OnImageAreaChanging(Nullable<global::System.Int32> value);
        partial void OnImageAreaChanged();

        #endregion

    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="TweetDataModel", Name="Tweet")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Tweet : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Tweet object.
        /// </summary>
        /// <param name="tweetID">Initial value of the TweetID property.</param>
        public static Tweet CreateTweet(global::System.String tweetID)
        {
            Tweet tweet = new Tweet();
            tweet.TweetID = tweetID;
            return tweet;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Text
        {
            get
            {
                return _Text;
            }
            set
            {
                OnTextChanging(value);
                ReportPropertyChanging("Text");
                _Text = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Text");
                OnTextChanged();
            }
        }
        private global::System.String _Text;
        partial void OnTextChanging(global::System.String value);
        partial void OnTextChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String TweetID
        {
            get
            {
                return _TweetID;
            }
            set
            {
                if (_TweetID != value)
                {
                    OnTweetIDChanging(value);
                    ReportPropertyChanging("TweetID");
                    _TweetID = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("TweetID");
                    OnTweetIDChanged();
                }
            }
        }
        private global::System.String _TweetID;
        partial void OnTweetIDChanging(global::System.String value);
        partial void OnTweetIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String LinkSite
        {
            get
            {
                return _LinkSite;
            }
            set
            {
                OnLinkSiteChanging(value);
                ReportPropertyChanging("LinkSite");
                _LinkSite = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LinkSite");
                OnLinkSiteChanged();
            }
        }
        private global::System.String _LinkSite;
        partial void OnLinkSiteChanging(global::System.String value);
        partial void OnLinkSiteChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("TweetDataModel", "LinkSite", "Website")]
        public Website Website
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Website>("TweetDataModel.LinkSite", "Website").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Website>("TweetDataModel.LinkSite", "Website").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Website> WebsiteReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Website>("TweetDataModel.LinkSite", "Website");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Website>("TweetDataModel.LinkSite", "Website", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="TweetDataModel", Name="Website")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Website : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Website object.
        /// </summary>
        /// <param name="url">Initial value of the Url property.</param>
        public static Website CreateWebsite(global::System.String url)
        {
            Website website = new Website();
            website.Url = url;
            return website;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                OnTitleChanging(value);
                ReportPropertyChanging("Title");
                _Title = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Title");
                OnTitleChanged();
            }
        }
        private global::System.String _Title;
        partial void OnTitleChanging(global::System.String value);
        partial void OnTitleChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url != value)
                {
                    OnUrlChanging(value);
                    ReportPropertyChanging("Url");
                    _Url = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }
        private global::System.String _Url;
        partial void OnUrlChanging(global::System.String value);
        partial void OnUrlChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("TweetDataModel", "LinkSite", "Tweet")]
        public EntityCollection<Tweet> Tweets
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Tweet>("TweetDataModel.LinkSite", "Tweet");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Tweet>("TweetDataModel.LinkSite", "Tweet", value);
                }
            }
        }

        #endregion

    }

    #endregion

    
}
