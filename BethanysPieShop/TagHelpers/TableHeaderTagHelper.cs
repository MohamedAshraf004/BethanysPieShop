using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BethanysPieShop.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("table",Attributes ="header")]
    public class TableHeaderTagHelper : TagHelper
    {
        public string HeaderContent { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            TagBuilder header = new TagBuilder("h2");
            header.InnerHtml.Append(HeaderContent);
            output.PreElement.SetHtmlContent(header);
        }
    }
}
