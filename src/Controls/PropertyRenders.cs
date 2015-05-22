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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace Zongsoft.Web.Controls
{
	public class BooleanPropertyRender : IPropertyRender
	{
		#region 单例字段
		public static readonly BooleanPropertyRender True = new BooleanPropertyRender(true);
		public static readonly BooleanPropertyRender False = new BooleanPropertyRender(false);
		#endregion

		#region 成员字段
		private bool _renderValue;
		#endregion

		#region 构造函数
		public BooleanPropertyRender(bool renderValue)
		{
			_renderValue = renderValue;
		}
		#endregion

		#region 公共属性
		public bool RenderValue
		{
			get
			{
				return _renderValue;
			}
			set
			{
				_renderValue = value;
			}
		}
		#endregion

		#region 公共方法
		public bool RenderProperty(HtmlTextWriter writer, PropertyMetadata property)
		{
			bool value;

			if(Zongsoft.Common.Convert.TryConvertValue(property.Value, out value) && (value == _renderValue))
				writer.AddAttribute(property.AttributeName, property.AttributeName);

			return true;
		}
		#endregion
	}

	public class UrlPropertyRender : IPropertyRender
	{
		#region 单例字段
		public static readonly UrlPropertyRender Default = new UrlPropertyRender();
		#endregion

		#region 公共方法
		public bool RenderProperty(HtmlTextWriter writer, PropertyMetadata property)
		{
			property.AttributeValue = property.Control.ResolveUrl(property.AttributeValue);
			return false;
		}
		#endregion
	}
}
