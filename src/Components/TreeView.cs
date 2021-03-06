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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Zongsoft.Web.Controls
{
	[DefaultProperty("Nodes")]
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class TreeView : CompositeDataBoundControl, INamingContainer
	{
		#region 成员字段
		private ITemplate _emptyTemplate;
		private ITemplate _nodeTemplate;
		private TreeViewNodeCollection _nodes;
		#endregion

		#region 构造函数
		public TreeView()
		{
			this.CssClass = "ui list";
		}
		#endregion

		#region 公共属性
		[DefaultValue(ListRenderMode.List)]
		[PropertyMetadata(false)]
		public ListRenderMode RenderMode
		{
			get
			{
				return this.GetPropertyValue(() => this.RenderMode);
			}
			set
			{
				this.SetPropertyValue(() => this.RenderMode, value);
			}
		}

		[DefaultValue(false)]
		[PropertyMetadata(false)]
		public bool IsDropdown
		{
			get
			{
				return this.GetPropertyValue(() => this.IsDropdown);
			}
			set
			{
				this.SetPropertyValue(() => this.IsDropdown, value);
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[PropertyMetadata(false)]
		public string LoadingPath
		{
			get
			{
				return this.GetPropertyValue(() => this.LoadingPath);
			}
			set
			{
				this.SetPropertyValue(() => this.LoadingPath, value);
			}
		}

		[Bindable(true)]
		[DefaultValue(ScrollbarMode.None)]
		[PropertyMetadata(false)]
		public ScrollbarMode ScrollbarMode
		{
			get
			{
				return this.GetPropertyValue(() => this.ScrollbarMode);
			}
			set
			{
				this.SetPropertyValue(() => this.ScrollbarMode, value);
			}
		}

		[Bindable(true)]
		[DefaultValue(SelectionMode.None)]
		[PropertyMetadata(false)]
		public SelectionMode SelectionMode
		{
			get
			{
				return this.GetPropertyValue(() => this.SelectionMode);
			}
			set
			{
				this.SetPropertyValue(() => this.SelectionMode, value);
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[PropertyMetadata(false)]
		public string SelectedPath
		{
			get
			{
				return this.GetPropertyValue(() => this.SelectedPath);
			}
			set
			{
				this.SetPropertyValue(() => this.SelectedPath, value);
			}
		}

		[PropertyMetadata(false)]
		public string DataPropertyName
		{
			get
			{
				return this.GetPropertyValue(() => this.DataPropertyName);
			}
			set
			{
				this.SetPropertyValue(() => this.DataPropertyName, value);
			}
		}

		[DefaultValue("item")]
		[PropertyMetadata(false)]
		public string ItemCssClass
		{
			get
			{
				return this.GetPropertyValue(() => this.ItemCssClass);
			}
			set
			{
				this.SetPropertyValue(() => this.ItemCssClass, value);
			}
		}

		[PropertyMetadata(false)]
		public string ListCssClass
		{
			get
			{
				return this.GetPropertyValue(() => this.ListCssClass);
			}
			set
			{
				this.SetPropertyValue(() => this.ListCssClass, value);
			}
		}

		[PropertyMetadata(false)]
		public bool HasNodes
		{
			get
			{
				return _nodes != null && _nodes.Count > 0;
			}
		}

		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewNodeCollection Nodes
		{
			get
			{
				if(_nodes == null)
					System.Threading.Interlocked.CompareExchange(ref _nodes, new TreeViewNodeCollection(this), null);

				return _nodes;
			}
		}

		[BrowsableAttribute(false)]
		[PersistenceModeAttribute(PersistenceMode.InnerProperty)]
		[TemplateContainerAttribute(typeof(TreeView))]
		public ITemplate EmptyTemplate
		{
			get
			{
				return _emptyTemplate;
			}
			set
			{
				_emptyTemplate = value;
			}
		}

		[BrowsableAttribute(false)]
		[PersistenceModeAttribute(PersistenceMode.InnerProperty)]
		[TemplateContainerAttribute(typeof(TreeView))]
		public ITemplate NodeTemplate
		{
			get
			{
				return _nodeTemplate;
			}
			set
			{
				_nodeTemplate = value;
			}
		}

		[Bindable(true)]
		[TypeConverter(typeof(UnitConverter))]
		[PropertyMetadata(false)]
		public Unit Height
		{
			get
			{
				return this.GetPropertyValue(() => this.Height);
			}
			set
			{
				this.SetPropertyValue(() => this.Height, value);
			}
		}

		[Bindable(true)]
		[TypeConverter(typeof(UnitConverter))]
		[PropertyMetadata(false)]
		public Unit Width
		{
			get
			{
				return this.GetPropertyValue(() => this.Width);
			}
			set
			{
				this.SetPropertyValue(() => this.Width, value);
			}
		}
		#endregion

		#region 公共方法
		public TreeViewNode Find(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var parts = path.Split('/');

			if(parts == null || parts.Length < 1)
				return null;

			TreeViewNode node = null;

			foreach(var part in parts)
			{
				if(string.IsNullOrWhiteSpace(part))
					continue;

				if(node == null)
					node = _nodes[part.Trim()];
				else
					node = node.Nodes[part.Trim()];

				if(node == null)
					return null;
			}

			return node;
		}
		#endregion

		#region 重写方法
		protected override void Render(HtmlTextWriter writer)
		{
			if((_nodes == null || _nodes.Count < 1) && this.IsEmptyDataSource)
			{
				if(_emptyTemplate != null)
				{
					_emptyTemplate.InstantiateIn(this);
					this.RenderChildren(writer);
				}

				return;
			}

			//调用基类同名方法
			base.Render(writer);
		}

		protected override void RenderBeginTag(HtmlTextWriter writer)
		{
			this.AddAttributes(writer);

			if(!Unit.IsEmpty(this.Height))
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());

			if(!Unit.IsEmpty(this.Width))
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Height.ToString());

			switch(this.ScrollbarMode)
			{
				case Web.Controls.ScrollbarMode.Horizontal:
					writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "scroll");
					break;
				case Web.Controls.ScrollbarMode.Vertical:
					writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "scroll");
					break;
				case Web.Controls.ScrollbarMode.Both:
					writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "scroll");
					break;
			}

			writer.RenderBeginTag(this.GetListTagName());
		}

		protected override void RenderEndTag(HtmlTextWriter writer)
		{
			writer.RenderEndTag();
		}

		protected override void RenderContent(HtmlTextWriter writer)
		{
			if(_nodes != null)
			{
				for(int i = 0; i < _nodes.Count; i++)
				{
					this.RenderNode(writer, _nodes[i], 0, i);
				}
			}

			if(string.IsNullOrWhiteSpace(this.LoadingPath))
				this.RenderDataNodes(writer, this.DataSource as IEnumerable, 0, 0, false);
		}
		#endregion

		#region 私有方法
		private void RenderNode(HtmlTextWriter writer, TreeViewNode node, int depth, int index = 0)
		{
			if(node == null || (!node.Visible))
				return;

			string cssClass = node.CssClass ?? this.ItemCssClass;

			if(!string.IsNullOrWhiteSpace(node.Name))
				writer.AddAttribute(HtmlTextWriterAttribute.Id, node.Name);

			if(node.Selected)
				cssClass = Utility.ResolveCssClass(":selected", () => cssClass);

			if(this.IsDropdown && node.Nodes.Count > 0)
				cssClass = Utility.ResolveCssClass(":ui dropdown", () => cssClass);

			if(!string.IsNullOrWhiteSpace(cssClass))
				writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

			writer.RenderBeginTag(this.GetItemTagName());

			if(!string.IsNullOrWhiteSpace(node.NavigateUrl))
			{
				if(!string.IsNullOrWhiteSpace(node.NavigateCssClass))
					writer.AddAttribute(HtmlTextWriterAttribute.Class, node.NavigateCssClass);

				writer.AddAttribute(HtmlTextWriterAttribute.Href, node.NavigateUrl == "#" ? Utility.EmptyLink : node.NavigateUrl);
				writer.RenderBeginTag(HtmlTextWriterTag.A);
			}

			if(node.Image != null)
				node.Image.ToHtmlString(writer);

			writer.WriteEncodedText(node.Text);

			if(!string.IsNullOrWhiteSpace(node.NavigateUrl))
				writer.RenderEndTag();

			if(node.Nodes.Count > 0)
			{
				cssClass = node.ListCssClass;

				if(this.IsDropdown)
					cssClass = Utility.ResolveCssClass(":menu", () => cssClass);

				if(!string.IsNullOrWhiteSpace(cssClass))
					writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

				writer.RenderBeginTag(this.GetListTagName());

				for(int i = 0; i < node.Nodes.Count; i++)
				{
					this.RenderNode(writer,
									node.Nodes[i],
									depth + 1, i);
				}

				writer.RenderEndTag();

				//添加下拉箭头图标
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon dropdown");
				writer.RenderBeginTag(HtmlTextWriterTag.I);
				writer.RenderEndTag();
			}

			if(!string.IsNullOrWhiteSpace(this.LoadingPath) && string.Equals(this.LoadingPath, node.FullPath, StringComparison.OrdinalIgnoreCase))
				this.RenderDataNodes(writer, this.DataSource as IEnumerable, depth, index, true);

			writer.RenderEndTag();
		}

		private void RenderDataNodes(HtmlTextWriter writer, IEnumerable dataItems, int depth, int index = 0, bool renderListTag = true)
		{
			if(dataItems == null || (dataItems is ICollection && ((ICollection)dataItems).Count < 1))
				return;

			if(Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(ICollection<>), dataItems.GetType()))
			{
				var property = dataItems.GetType().GetProperty("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if(property != null && Zongsoft.Common.Convert.ConvertValue<int>(property.GetValue(dataItems)) < 1)
					return;
			}

			if(renderListTag)
			{
				var cssClass = this.ListCssClass ?? this.ListCssClass;

				if(this.IsDropdown)
					cssClass = Utility.ResolveCssClass(":menu", () => cssClass);

				if(!string.IsNullOrWhiteSpace(cssClass))
					writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

				writer.RenderBeginTag(this.GetListTagName());
			}

			foreach(var dataItem in dataItems)
			{
				FormExtension.PushDataItem(this.Page, dataItem, index);

				this.RenderNodeTemplate(writer, new TreeViewNodeContainer(this, dataItem, index++, this.GetItemTagName(), this.ItemCssClass)
				{
					Depth = depth,
				});

				FormExtension.PopDataItem(this.Page);
			}

			if(renderListTag)
				writer.RenderEndTag();
		}

		private void RenderNodeTemplate(HtmlTextWriter writer, TreeViewNodeContainer container)
		{
			if(_nodeTemplate != null)
			{
				_nodeTemplate.InstantiateIn(container);

				if(Utility.GetVisibleChildrenCount(container) > 0)
					container.RenderControl(writer);
			}
		}

		private string GetListTagName()
		{
			switch(this.RenderMode)
			{
				case ListRenderMode.List:
					return "dl";
				case ListRenderMode.BulletList:
					return "ul";
				case ListRenderMode.OrderedList:
					return "ol";
			}

			return "div";
		}

		private string GetItemTagName()
		{
			switch(this.RenderMode)
			{
				case ListRenderMode.List:
					return "dt";
				case ListRenderMode.BulletList:
				case ListRenderMode.OrderedList:
					return "li";
			}

			return "div";
		}

		private bool IsEmptyDataSource
		{
			get
			{
				var dataSource = this.DataSource;

				if(dataSource == null)
					return true;

				var collection = dataSource as ICollection;
				return collection == null || collection.Count < 1;
			}
		}
		#endregion

		#region 嵌套子类
		internal class TreeViewNodeContainer : DataItemContainer<TreeView>
		{
			#region 成员字段
			private int _depth;
			#endregion

			#region 构造函数
			internal TreeViewNodeContainer(TreeView owner, object dataItem, int index, string tagName = null, string cssClass = null)
				: base(owner, dataItem, index, index, tagName, cssClass)
			{
			}

			internal TreeViewNodeContainer(TreeView owner, object dataItem, int index, int displayIndex, string tagName = null, string cssClass = null)
				: base(owner, dataItem, index, displayIndex, tagName, cssClass)
			{
			}
			#endregion

			#region 公共属性
			public int Depth
			{
				get
				{
					return _depth;
				}
				internal set
				{
					_depth = value;
				}
			}
			#endregion

			#region 重写方法
			protected override void RenderContent(HtmlTextWriter writer)
			{
				//调用基类同名方法(生成当前节点的内容)
				base.RenderContent(writer);

				if(string.IsNullOrWhiteSpace(this.Owner.DataPropertyName))
					return;

				var dataProperty = this.DataItem.GetType().GetProperty(this.Owner.DataPropertyName, (BindingFlags.Instance | BindingFlags.Public));

				if(dataProperty == null)
					throw new InvalidOperationException();

				this.Owner.RenderDataNodes(writer, dataProperty.GetValue(this.DataItem) as IEnumerable, _depth + 1, 0, true);
			}
			#endregion
		}
		#endregion
	}
}
