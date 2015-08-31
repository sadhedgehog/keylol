﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Keylol.Models
{
    public enum LanguageConversionMode
    {
        ForceSimplifiedChinese,
        ForceTraditionalChinese,
        SimplifiedChineseWithContentUnmodified,
        TraditionalChineseWithContentUnmodified
    }

    // You can add profile data for the user by adding more properties to your KeylolUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public sealed class KeylolUser : IdentityUser
    {
        public KeylolUser()
        {
            LockoutEnabled = true;
        }

        public KeylolUser(string userName) : this()
        {
            UserName = userName;
        }

        [Required]
        [MaxLength(5)]
        public string IdCode { get; set; }

        public DateTime RegisterTime { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(64)]
        public string RegisterIp { get; set; }

        public ICollection<Point> SubscribedPoints { get; set; }
        public ProfilePoint ProfilePoint { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<UserMessage> SentUserMessages { get; set; }
        public ICollection<SystemMessageWarningNotification> SentWarnings { get; set; }
        public ICollection<NormalPoint> ModeratedPoints { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<KeylolUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        #region Preferences

        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string GamerTag { get; set; } = string.Empty;

        // Auto share options
        public bool AutoShareOnAcquiringNewGame { get; set; } = true;
        public bool AutoShareOnPublishingReview { get; set; } = true;
        public bool AutoShareOnUnlockingAchievement { get; set; } = true;
        public bool AutoShareOnUploadingScreenshot { get; set; } = true;
        public bool AutoShareOnAddingFavorite { get; set; } = true;

        // Email notification options
        public bool EmailNotifyOnArticleReplied { get; set; } = true;
        public bool EmailNotifyOnCommentReplied { get; set; } = true;
        public bool EmailNotifyOnEditorRecommended { get; set; } = true;
        public bool EmailNotifyOnUserMessageReceived { get; set; } = true;

        public LanguageConversionMode PreferedLanguageConversionMode { get; set; } = LanguageConversionMode.SimplifiedChineseWithContentUnmodified;

        // Accessibility demand
        public bool ColorVisionDeficiency { get; set; } = false;
        public bool VisionImpairment { get; set; } = false;
        public bool HearingImpairment { get; set; } = false;
        public bool PhotosensitiveEpilepsy { get; set; } = false;

        #endregion
    }
}