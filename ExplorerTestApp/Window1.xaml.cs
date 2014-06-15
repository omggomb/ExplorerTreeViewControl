﻿/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 06/02/2014
 * Time: 12:50
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ExplorerTreeView;

using OmgUtils.UserInteraction;

namespace ExplorerTestApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			
			explorerTreeView.InitializeTree(new ItemFactory());
			

			                                          
			                                  		
		}
	}
}