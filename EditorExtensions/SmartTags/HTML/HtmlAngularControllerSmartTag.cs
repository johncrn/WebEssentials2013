﻿using Microsoft.Html.Core;
using Microsoft.Html.Editor.SmartTags;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MadsKristensen.EditorExtensions.SmartTags
{
    [Export(typeof(IHtmlSmartTagProvider))]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    [Order(Before = "Default")]
    [Name("HtmlAngularController")]
    internal class HtmlAngularControllerSmartTagProvider : IHtmlSmartTagProvider
    {
        public IHtmlSmartTag TryCreateSmartTag(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute, int caretPosition, HtmlPositionType positionType)
        {
            if (element.GetAttribute("ng-controller") != null)
            {
                return new HtmlAngularControllerSmartTag(textView, textBuffer, element);
            }

            return null;
        }
    }

    internal class HtmlAngularControllerSmartTag : HtmlSmartTag
    {
        public HtmlAngularControllerSmartTag(ITextView textView, ITextBuffer textBuffer, ElementNode element)
            : base(textView, textBuffer, element, HtmlSmartTagPosition.ElementName)
        {
        }

        protected override IEnumerable<ISmartTagAction> GetSmartTagActions(ITrackingSpan span)
        {
            return new ISmartTagAction[] { new FormatSelectionSmartTagAction(this) };
        }

        class FormatSelectionSmartTagAction : HtmlSmartTagAction
        {
            public FormatSelectionSmartTagAction(HtmlSmartTag htmlSmartTag) :
                base(htmlSmartTag, "Add new Angular controller")
            { }

            public override void Invoke()
            {
                string value = HtmlSmartTag.Element.GetAttribute("ng-controller").Value;

                if (string.IsNullOrEmpty(value))
                    value = "myController";

                string folder = ProjectHelpers.GetProjectFolder(EditorExtensionsPackage.DTE.ActiveDocument.FullName);
                string file;

                using (var dialog = new SaveFileDialog())
                {
                    dialog.FileName = value + ".js";
                    dialog.DefaultExt = ".js";
                    dialog.Filter = "JS files | *.js";
                    dialog.InitialDirectory = folder;

                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    file = dialog.FileName;
                }                
                                               
                EditorExtensionsPackage.DTE.UndoContext.Open(this.DisplayText);
                
                string script = GetScript(value);
                File.WriteAllText(file, script);
                
                ProjectHelpers.AddFileToActiveProject(file);
                EditorExtensionsPackage.DTE.ItemOperations.OpenFile(file);

                EditorExtensionsPackage.DTE.UndoContext.Close();
            }

            private static string GetScript(string value)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("var myApp = angular.module('myApp',[]);");
                sb.AppendLine();
                sb.AppendLine("myApp.controller('{0}', ['$scope', function($scope) {{");
                sb.AppendLine("    $scope.greeting = 'Hola!!';");
                sb.AppendLine("}}]);");

                string script = string.Format(sb.ToString(), value);
                return script;
            }
        }
    }
}
