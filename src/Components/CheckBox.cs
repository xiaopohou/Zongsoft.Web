﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Zongsoft.Web.Controls
{
	public class CheckBox : InputBox
	{
		#region 构造函数
		public CheckBox() : base(InputBoxType.CheckBox)
		{
		}
		#endregion

		#region 公共属性
		[Bindable(true)]
		[DefaultValue(false)]
		[PropertyMetadata(PropertyRender = "BooleanPropertyRender.True")]
		public bool Checked
		{
			get
			{
				return this.GetPropertyValue(() => this.Checked);
			}
			set
			{
				this.SetPropertyValue(() => this.Checked, value);
			}
		}

		[DefaultValue(false)]
		[PropertyMetadata(false)]
		public bool IsRadio
		{
			get
			{
				return this.InputType == InputBoxType.Radio;
			}
			set
			{
				this.InputType = value ? InputBoxType.Radio : InputBoxType.CheckBox;
			}
		}
		#endregion

		#region 重写属性
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(InputBoxType.CheckBox)]
		public override InputBoxType InputType
		{
			get
			{
				return base.InputType;
			}
			set
			{
				if(value != InputBoxType.CheckBox && value != InputBoxType.Radio)
					throw new NotSupportedException();

				base.InputType = value;
			}
		}
		#endregion
	}
}
