using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace iGEM_Db_JSON
{
    class Program
    {

        #region AlphaBrick_JSON_Classes

        private class abPart
        {
            public string part_name { get; set; }
            public string part_short_name { get; set; }
            public string part_short_desc { get; set; }
            public string part_type { get; set; }
            public string part_url { get; set; }
            public abSequence sequences { get; set; }
            public abFeature features { get; set; }
        }

        // TODO: Consider reformatting this - remove?
        private class abSequence
        {
            public string seq_data { get; set; }
        }

        // TODO: Consider reformatting this.
        private class abFeature
        {
            public List<abFeatureActual> feature;
        }

        private class abFeatureActual
        {
            public string id { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string direction { get; set; }
            public string startpos { get; set; }
            public string endpos { get; set; }
        }

        #endregion

        #region JSONMaker

        /// <summary>
        /// Makes AlphaBrick's JSON format from igem_simplified SQL database data (parts and features tables).
        /// </summary>
        private static class JSONMaker
        {

            // Set up EF, prepare main variables.
            private static igem_simplifiedEntities igemDb = new igem_simplifiedEntities();
            private static int partsCount = 0;
            private static JavaScriptSerializer serializer = new JavaScriptSerializer();
            private static List<abPart> abPartList = new List<abPart>();

            /// <summary>
            /// A method to makes AlphaBrick's JSON format from igem_simplified SQL database data (parts and features tables).
            /// </summary>
            /// <param name="partsLimit">Optional limit on the number of parts to include. Default or 0 means all will be included.</param>
            /// <returns>The AlphaBrick's JSON format JSON string.</returns>
            public static string makeJSON(int partsLimit = 0)
            {

                int maxParts = igemDb.parts.Count();
                if (partsLimit <= 0 || partsLimit > maxParts) partsLimit = maxParts;

                foreach (parts part in igemDb.parts)
                {

                    if (partsCount < partsLimit)
                    {

                        abPart currAbPart = new abPart();
                        abSequence currAbSeqequence = new abSequence();
                        abFeature currAbFeature = new abFeature();
                        List<abFeatureActual> currAbFeatureActualList = new List<abFeatureActual>();

                        partsCount++;
                        Console.Write("\r{0} records.   ", "Processed " + partsCount);
                        //Console.WriteLine("Processing: " + part.part_name);

                        // TODO: Consider changing all this.
                        currAbPart.part_name = part.part_name;
                        currAbPart.part_short_desc = part.short_desc;
                        currAbPart.part_short_name = part.nickname;
                        currAbPart.part_type = part.part_type;
                        currAbPart.part_url = "http://parts.igem.org/Part:" + currAbPart.part_name;  // NOTE: Manually generated.

                        // TODO: Consider adding sequence length.
                        currAbSeqequence.seq_data = part.sequence;
                        currAbPart.sequences = currAbSeqequence;

                        List<features> featureCollection = new List<features>();

                        // EF issue, can't just do part.feature for some reason, so doing a workaround like this!
                        igem_simplifiedEntities igemDb2 = new igem_simplifiedEntities();
                        featureCollection = igemDb2.features.Where(x => x.part_id == part.part_id).ToList();

                        foreach (features feature in featureCollection)
                        {

                            abFeatureActual currAbFeatureActual = new abFeatureActual();

                            currAbFeatureActual.id = feature.feature_id.ToString();
                            currAbFeatureActual.title = feature.label;
                            currAbFeatureActual.type = feature.feature_type;

                            // NOTE: Converted from "Boolean" to text.
                            // TODO: Consider changing this.
                            string dir = "forward";
                            if (feature.direction_is_reverse == 1) dir = "reverse";
                            currAbFeatureActual.direction = dir;

                            currAbFeatureActual.startpos = feature.start_pos.ToString();
                            currAbFeatureActual.endpos = feature.end_pos.ToString();

                            // Add to the "array" of Features.
                            currAbFeatureActualList.Add(currAbFeatureActual);

                        }
                        currAbFeature.feature = currAbFeatureActualList;

                        // Add to the "array" of Parts.
                        abPartList.Add(currAbPart);

                    }

                }

                serializer.MaxJsonLength = Int32.MaxValue;
                string output = serializer.Serialize(abPartList);
                Console.WriteLine("\n" + "The length of the JSON is: " + output.Length + " characters.");

                return output;

            }

        }

        #endregion

        #region Main

        /// <summary>
        /// Main program body.
        /// </summary>
        /// <param name="args">Arguments.</param>
        static void Main(string[] args)
        {

            string JSONOutput = "";

            /*try
            {*/

            Console.WriteLine("Converting igem_simplified SQL data to AlphaBrick's JSON...");

            JSONOutput = JSONMaker.makeJSON();

            Console.WriteLine(@"Saving JSON file to C:\Temp\ ...");

            File.Delete(@"C:\Temp\igem_alphabrick.json");
            File.WriteAllText(@"C:\Temp\igem_alphabrick.json", JSONOutput);

            Console.WriteLine(@"File saved successfully.");

            /*}
            catch(Exception e)
            {
                Console.WriteLine("An error has occurred. Error message is:");
                Console.WriteLine(e.Message);
            }*/

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

        }

        #endregion

    }
}
