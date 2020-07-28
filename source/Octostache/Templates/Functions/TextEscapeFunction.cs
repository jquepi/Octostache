using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Markdig;

namespace Octostache.Templates.Functions
{
    internal class TextEscapeFunction
    {

        public static string HtmlEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            return Escape(argument, HtmlEntityMap);
        }

        public static string XmlEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            return Escape(argument, XmlEntityMap);
        }

        public static string JsonEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            return Escape(argument, JsonEntityMap);
        }

        public static string YamlSingleQuoteEscape(string argument, string[] options)
        {
            if (argument == null || options.Any())
                return null;

            argument = ReplaceNewLinesWithDoubleNewLines(argument);
            
            return Escape(argument, YamlSingleQuoteMap);
        }

        public static string YamlDoubleQuoteEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            return Escape(argument, YamlDoubleQuoteMap);
        }

        private static readonly Regex NewLineRegex = new Regex(@"\r?\n");
        
        private static string ReplaceNewLinesWithDoubleNewLines(string input)
        {
            return NewLineRegex.Replace(input, "$0$0");
        }
        
        [Obsolete("Please use MarkdownToHtml instead.")]
        public static string Markdown(string argument, string[] options)
        {
            return MarkdownToHtml(argument, options);
        }

        public static string MarkdownToHtml(string argument, string[] options)
        {
            if (argument == null || options.Any())
                return null;

            var pipeline = new MarkdownPipelineBuilder()
                .UsePipeTables()
                .UseEmphasisExtras() //strike through, subscript, superscript
                .UseAutoLinks()      //make links for http:// etc
                .Build();
            return Markdig.Markdown.ToHtml(argument.Trim(), pipeline) + '\n';
        }

        public static string UriStringEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            if (argument == null)
                return null;

            return Uri.EscapeUriString(argument);
        }

        public static string UriDataStringEscape(string argument, string[] options)
        {
            if (options.Any())
                return null;

            if (argument == null)
                return null;

            return Uri.EscapeDataString(argument);
        }

        static string Escape(string raw, IDictionary<char, string> entities)
        {
            if (raw == null)
                return null;

            return string.Join("", raw.Select(c =>
            {
                string entity;
                if (entities.TryGetValue(c, out entity))
                    return entity;
                return c.ToString();
            }));
        }

        static readonly IDictionary<char, string> HtmlEntityMap = new Dictionary<char, string>
        {
            { '&', "&amp;" },
            { '<', "&lt;" },
            { '>', "&gt;" },
            { '"', "&quot;" },
            { '\'', "&apos;" },
            { '/', "&#x2F;" }
        };

        static readonly IDictionary<char, string> XmlEntityMap = new Dictionary<char, string>
        {
            { '&', "&amp;" },
            { '<', "&lt;" },
            { '>', "&gt;" },
            { '"', "&quot;" },
            { '\'', "&apos;" }
        };

        // This is overly simplistic since Unicode chars also need escaping.
        static readonly IDictionary<char, string> JsonEntityMap = new Dictionary<char, string>
        {
            { '\"', @"\""" },
            { '\r', @"\r" },
            { '\t', @"\t" },
            { '\n', @"\n" },
            { '\\', @"\\" }
        };
        
        static readonly IDictionary<char, string> YamlSingleQuoteMap = new Dictionary<char, string>
        {
            { '\'', "''" }
        };
        
        static readonly IDictionary<char, string> YamlDoubleQuoteMap = new Dictionary<char, string>
        {
            { '\\', "\\\\"},
            { '"', "\\\""}
        };
    }
}