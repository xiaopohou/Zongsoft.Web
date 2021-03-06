﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Web.
 *
 * Zongsoft.Web is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Web is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Web; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Zongsoft.Security;
using Zongsoft.Security.Membership;

namespace Zongsoft.Web.Security
{
	public class AuthenticationFilter : System.Web.Mvc.Filters.IAuthenticationFilter
	{
		#region 成员字段
		private ICredentialProvider _credentialProvider;
		#endregion

		#region 公共属性
		[Zongsoft.Services.ServiceDependency]
		public ICredentialProvider CredentialProvider
		{
			get
			{
				return _credentialProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_credentialProvider = value;
			}
		}
		#endregion

		#region 验证实现
		public void OnAuthentication(AuthenticationContext filterContext)
		{
			var credentialId = AuthenticationUtility.CredentialId;

			if(string.IsNullOrWhiteSpace(credentialId))
				filterContext.Principal = CredentialPrincipal.Empty;
			else
				filterContext.Principal = new CredentialPrincipal(new CredentialIdentity(credentialId, this.CredentialProvider));
		}

		public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
		{
			if(AuthenticationUtility.IsAuthenticated || AuthenticationUtility.GetAuthorizationMode(filterContext.ActionDescriptor) == AuthorizationMode.Disabled)
				return;

			var url = Utility.RepairQueryString(Zongsoft.Web.Security.AuthenticationUtility.GetLoginUrl(), filterContext.HttpContext.Request.Url.Query);
			url = Utility.RepairQueryString(url, "?ReturnUrl=" + Uri.EscapeDataString(filterContext.HttpContext.Request.RawUrl));
			filterContext.Result = new RedirectResult(url);
		}
		#endregion
	}
}
