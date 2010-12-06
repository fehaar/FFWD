using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.Tentacles.Scripts
{
    //[DelimitedRecord(",")]
    public class LevelImportDataStructure
    {
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string world_id;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string world_name;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string level_id;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string theme_id;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string theme_name;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string batch;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string scene_name;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string level_name;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string order;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string gold_score;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string silver_score;
        //[FieldQuoted('"', QuoteMode.OptionalForRead)]
        public string bronze_score;
    }
}
