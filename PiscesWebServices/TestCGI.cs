﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Reclamation.Core;
using Reclamation.TimeSeries;
using System.IO;

namespace PiscesWebServices
{
    [TestFixture]    
    class TestCGI
    {
        public static void Main()
        {
            //Logger.EnableLogger();
            TestCGI t = new TestCGI();
            t.PerfTest();
        }

        [Test]
        public void PerfTest()
        {
            string payload = "parameter=mddo ch,wcao q,boii Z,boii ob,&syer=2015&smnth=4&sdy=13&eyer=2015&emnth=9&edy=1&format=2";
            RunTest(payload);
        }
        [Test]
        public void PerfTest2()
        {
            string payload = "parameter=mddo ch,wcao q,boii Z,boii ob,&syer=2015&smnth=4&sdy=13&eyer=2015&emnth=4&edy=13&format=2";
            RunTest(payload);
        }


        private static void RunTest(string payload)
        {
            Performance p = new Performance();
            TimeSeriesDatabase db = TimeSeriesDatabase.InitDatabase(new Arguments(new string[] { }));
            CsvTimeSeriesWriter c = new CsvTimeSeriesWriter(db);
            var fn = FileUtility.GetTempFileName(".txt");
            fn = "";
            c.Run(TimeInterval.Hourly, payload, fn);

            if (File.Exists(fn))
                p.Report(File.ReadAllLines(fn).Length + " lines read");
            else
                p.Report();
        }



        [Test]
        public void CompareLinuxToVMSCGI()
        {
            //http://www.usbr.gov/pn-bin/webdaycsv.pl?parameter=mddo%20ch,wcao%20q&syer=2015&smnth=4&sdy=5&eyer=2015&emnth=4&edy=5&format=2
            string payload = "parameter=mddo ch,wcao q,boii Z,boii ob,&syer=2015&smnth=4&sdy=13&eyer=2015&emnth=4&edy=13&format=2";
            //Program.Main(new string[] { "--cgi=instant", "--payload=?"+payload });

            TimeSeriesDatabase db = TimeSeriesDatabase.InitDatabase(new Arguments(new string[]{}));
            CsvTimeSeriesWriter c = new CsvTimeSeriesWriter(db);
            var fn = FileUtility.GetTempFileName(".txt");
            c.Run( TimeInterval.Hourly, payload,fn);

            TextFile tf = new TextFile(fn);
            tf.DeleteLines(0, 1);

            var fnhyd0 = FileUtility.GetTempFileName(".txt");
            Web.GetFile("http://www.usbr.gov/pn-bin/webdaycsv.pl?" + payload, fnhyd0);

          var diff = TextFile.Compare(tf, new TextFile(fnhyd0));
          if (diff.Length > 0)
          {
              for (int i = 0; i < diff.Length; i++)
              {
                  Console.WriteLine(diff[i]);    
              }
              
          }

        }

        [Test]
        public void DumpTest()
        {
            Program.Main(new string[] { "--cgi=sites" });
        }
    }
}
