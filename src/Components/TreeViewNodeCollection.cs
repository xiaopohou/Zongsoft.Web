﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.ObjectModel;
using System.Text;

namespace Zongsoft.Web.Controls
{
	public class TreeViewNodeCollection : Zongsoft.Collections.NamedCollectionBase<TreeViewNode>
	{
		#region 同步变量
		private readonly object _syncRoot;
		#endregion

		#region 成员变量
		private TreeView _treeView;
		private TreeViewNode _parent;
		#endregion

		#region 构造函数
		internal TreeViewNodeCollection(TreeView owner, TreeViewNode parent = null) : base(StringComparer.OrdinalIgnoreCase)
		{
			//if(owner == null)
			//	throw new ArgumentNullException("owner");

			_treeView = owner;
			_parent = parent;
			_syncRoot = new object();
		}
		#endregion

		#region 公共属性
		public TreeView TreeView
		{
			get
			{
				return _treeView;
			}
		}

		public TreeViewNode Parent
		{
			get
			{
				return _parent;
			}
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(TreeViewNode item)
		{
			return item.Name;
		}

		protected override void SetItem(int index, TreeViewNode item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			if(item.Parent != null)
				throw new ArgumentException();

			item.TreeView = _treeView;
			item.Parent = _parent;

			//调用基类同名方法
			base.SetItem(index, item);
		}

		protected override void InsertItems(int index, IEnumerable<TreeViewNode> items)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			foreach(var item in items)
			{
				if(item.Parent != null)
					throw new ArgumentException();

				item.TreeView = _treeView;
				item.Parent = _parent;
			}

			//使用同步锁，以确保不与删除和清除方法冲突
			lock(_syncRoot)
			{
				//调用基类同名方法
				base.InsertItems(index, items);
			}
		}

		protected override void RemoveItem(int index)
		{
			lock(_syncRoot)
			{
				var item = base[index];

				if(item != null)
				{
					item.TreeView = null;
					item.Parent = null;
				}

				//调用基类同名方法
				base.RemoveItem(index);
			}
		}

		protected override void ClearItems()
		{
			lock(_syncRoot)
			{
				foreach(var item in base.Items)
				{
					if(item != null)
					{
						item.TreeView = null;
						item.Parent = null;
					}
				}

				//调用基类同名方法
				base.ClearItems();
			}
		}
		#endregion
	}
}
