﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MadsKristensen.EditorExtensions
{
    public class Microdata : IRule
    {
        public string Message
        {
            get { return "SEO: Use HTML5 microdata to add semantic meaning to the website."; }
        }

        public string Question
        {
            get { return "Do you want to browse to a tutorial?"; }
        }

        public TaskErrorCategory Category
        {
            get { return TaskErrorCategory.Message;  }
        }

        public void Navigate(object sender, EventArgs e)
        {
            ErrorTask task = (ErrorTask)sender;

            if (MessageBox.Show(Question, "Web Essentials", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var process = Process.Start("http://www.seomoves.org/blog/build/html5-microdata-2711/");
            }
        }
    }
}