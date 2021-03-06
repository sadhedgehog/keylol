﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keylol.Models
{
    public class CouponLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Index(IsUnique = true, IsClustered = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long Sid { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual KeylolUser User { get; set; }

        public CouponEvent Event { get; set; }

        public int Change { get; set; }

        public int Balance { get; set; }

        [Index]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        ///     变动描述，使用 JSON 格式
        /// </summary>
        [Required]
        public string Description { get; set; }
    }

    public enum CouponEvent
    {
        新注册,
        应邀注册,
        发布文章,
        [Obsolete] 发布简评,
        发出认可,
        获得认可,
        每日访问,
        邀请注册,
        其他,
        兑换商品
    }
}