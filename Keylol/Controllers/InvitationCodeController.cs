﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Keylol.Models;
using Keylol.Models.DTO;
using Keylol.Utilities;
using Swashbuckle.Swagger.Annotations;

namespace Keylol.Controllers
{
    [Authorize]
    [RoutePrefix("invitation-code")]
    public class InvitationCodeController : KeylolApiController
    {
        /// <summary>
        /// 验证一个邀请码是否正确
        /// </summary>
        /// <param name="code">邀请码</param>
        [AllowAnonymous]
        [Route("{code}")]
        [ResponseType(typeof (InvitationCodeDTO))]
        [SwaggerResponse(HttpStatusCode.NotFound, "邀请码无效")]
        public async Task<IHttpActionResult> Get(string code)
        {
            var c = await DbContext.InvitationCodes.FindAsync(code);
            if (c == null || c.UsedByUser != null)
                return NotFound();
            return Ok(new InvitationCodeDTO(c));
        }

        /// <summary>
        /// 获取未使用的邀请码列表
        /// </summary>
        /// <param name="source">邀请码来源，不填表示获取所有来源的邀请码，默认 null</param>
        /// <param name="skip">起始位置，默认 0</param>
        /// <param name="take">获取数量，默认 50，最大 2000</param>
        [ClaimsAuthorize(StaffClaim.ClaimType, StaffClaim.Operator)]
        [Route]
        [ResponseType(typeof (List<InvitationCodeDTO>))]
        public async Task<IHttpActionResult> GetList(string source = null, int skip = 0, int take = 50)
        {
            if (take > 2000) take = 2000;
            var query = DbContext.InvitationCodes.Where(c => c.UsedByUser == null);
            if (source != null)
                query = query.Where(c => c.Source == source);
            return
                Ok((await query.OrderBy(c => c.GenerateTime).Skip(() => skip).Take(() => take).ToListAsync())
                    .Select(c => new InvitationCodeDTO(c, true)));
        }


        /// <summary>
        /// 生成邀请码
        /// </summary>
        /// <param name="source">邀请码来源，用于统计追踪</param>
        /// <param name="number">生成数量，最大 20000，默认 1</param>
        [ClaimsAuthorize(StaffClaim.ClaimType, StaffClaim.Operator)]
        [Route]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof (List<InvitationCodeDTO>))]
        public async Task<IHttpActionResult> Post(string source, int number = 1)
        {
            if (number > 20000) number = 20000;
            var codes = Enumerable.Range(0, number).Select(i => new InvitationCode {Source = source}).ToList();
            DbContext.InvitationCodes.AddRange(codes);
            await DbContext.SaveChangesAsync();
            return Created("invitation-code", codes.Select(c => new InvitationCodeDTO(c, true)));
        }
    }
}