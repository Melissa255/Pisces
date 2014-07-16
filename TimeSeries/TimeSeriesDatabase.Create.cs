﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reclamation.Core;

namespace Reclamation.TimeSeries
{
    public partial class TimeSeriesDatabase
    {

        private void ExecuteCreateTable(BasicDBServer svr , string sql)
        {
            svr.CreateTable(sql);
        }

        private void CreateTablesWithSQL()
        {
            CreatePiscesInfoTable();
            CreateSeriesCatalogTable();
            CreateScenarioTable();
            CreateLimitTable();
            CreateSiteTable();
            CreateSitePropertiesTable();
            CreateSiteDetailsTable();
            CreateSeriesPropertiesTable();

            UpgradeDatabase();

            //CreateCalculationTable();
        }

        private void CreateSiteTable()
        {
            if (!m_server.TableExists("sitecatalog"))
            {
                string sql = "Create Table sitecatalog "
                + "( siteid  " + m_server.PortableCharacterType(256) + " not null primary key, "
                + " description " + m_server.PortableCharacterType(1024) + " not null default '', "
                + " state " + m_server.PortableCharacterType(30) + " not null default '', "
                + " latitude " + m_server.PortableCharacterType(30) + " not null default '', "
                + " longitude " + m_server.PortableCharacterType(30) + " not null default '', "
                + " elevation " + m_server.PortableCharacterType(30) + " not null default '', "
                + " timezone " + m_server.PortableCharacterType(30) + " not null default '', "
                + " install " + m_server.PortableCharacterType(30) + " not null default '' "
                + " )";
                ExecuteCreateTable(m_server, sql);

            } 
        }

        private void CreateSitePropertiesTable()
        {
            if (!m_server.TableExists("siteproperties"))
            {
                string sql = "Create Table siteproperties "
                               + "( id int not null primary key, "
                               + " siteid " + m_server.PortableCharacterType(256) + " not null default '' , "
                               + " name " + m_server.PortableCharacterType(1024) + " not null default '', "
                               + " value " + m_server.PortableCharacterType(10) + " not null default '' "
                               + " )";
                ExecuteCreateTable(m_server, sql);

            } 

        }

        private void CreateSeriesPropertiesTable()
        {
            if (!m_server.TableExists("seriesproperties"))
            {
                string sql = "Create Table seriesproperties "
                + "( id int not null primary key, "
                + " seriesid int not null default 0, " // + m_server.PortableCharacterType(256) + " not null default 0, "
                + " name " + m_server.PortableCharacterType(1024) + " not null default '', "
                + " value " + m_server.PortableCharacterType(10) + " not null default '' "


                + " )";
                ExecuteCreateTable(m_server, sql);

            }
        }

        private void CreateSiteDetailsTable()
        {
            //if (!m_server.TableExists("sitedetails"))
            //{
            //    string sql = "Create Table sitedetails "
            //    + "( siteid  " + m_server.PortableCharacterType(256) + " not null primary key, "
            //    + " name " + m_server.PortableCharacterType(1024) + " not null default '', "
            //    + " value " + m_server.PortableCharacterType(10) + " not null default '' "


            //    + " )";
            //    ExecuteCreateTable(m_server, sql);

            //}
        }

        private void CreatePiscesInfoTable()
        {
            if (!m_server.TableExists("piscesinfo"))
            {
                string sql = "Create Table piscesinfo "
                + "( name  "+m_server.PortableCharacterType(256)+" not null primary key, "
                + " value "+m_server.PortableCharacterType(1024)+" not null default '' "
                + " )";
                ExecuteCreateTable(m_server, sql);

                
                sql = "insert INTO piscesinfo values ('FileVersion', '2')";
                m_server.RunSqlCommand(sql);
            }

            InitSettings();
            if (m_settings.GetDBVersion() != 2)
            {
                m_settings.Set("FileVersion", 2);
                m_settings.Save();
            }
        }



        void CreateSeriesTable(string tableName, bool hasFlags)
        {
            string dataType = "float";// "real"; //"float"; // double percision

            string sql = "Create Table " + m_server.PortableTableName(tableName);
            if (!hasFlags)
            {
                sql += "( datetime " + m_server.PortableDateTimeType() + " primary key, value " + dataType + " )";
            }
            else
            {
                sql += "( datetime " + m_server.PortableDateTimeType() + " primary key, value " + dataType + ", flag " + m_server.PortableCharacterType(50) + " )";
            }
            ExecuteCreateTable(m_server,sql);
        }

        ///// <summary>
        ///// Create list of Dates to simplify queries
        ///// </summary>
        //private void CreateCalendarTables()
        //{
        //    if (m_server.TableExists("DailyCalendar"))
        //        return;
        //    var sql = "Create Table [DailyCalendar] "
        //        + " ( DateTime DateTime primary key  )";

        //    m_server.RunSqlCommand(sql);

        //    var tbl = m_server.Table("DailyCalendar");
        //    DateTime t = MinDateTime.Date;
        //    DateTime t2 = new DateTime(2100,12,31);
        //    while (t < t2)
        //    {
        //        tbl.Rows.Add(t);
        //        t = t.AddDays(1).Date;
        //    }
        //    m_server.SaveTable(tbl);
        //}


        private void CreateScenarioTable()
        {
            if (m_server.TableExists("scenario"))
                return;

            string sql = "Create Table scenario "
                         + " ( "
                         + " sortorder int not null primary key default 0,"
                         + " name " + m_server.PortableCharacterType(200) + " not null default '',"
                         + "  path " + m_server.PortableCharacterType(1024) + " not null default '', "
                         + "  checked smallint not null default 0"
                         + "  )";
            ExecuteCreateTable(m_server, sql);
        }



        private void CreateSeriesCatalogTable()
        {
            if (m_server.TableExists("seriescatalog"))
            {
                return;
            }
            string sql = "Create Table seriescatalog "
                + " ( "
                + " id int not null primary key ,"
                + " parentid int not null default 0, "
                + " isfolder smallint not null default 0, "
                + " sortorder int not null default 0,"
                + " iconname " + m_server.PortableCharacterType(100) + " not null default '',"
                + " name " + m_server.PortableCharacterType(200) + " not null default '',"
                + " siteid " + m_server.PortableCharacterType(2600) + " not null default '',"
                + " units " + m_server.PortableCharacterType(100) + " not null default '',"
                + " timeinterval " + m_server.PortableCharacterType(100) + " not null default 'irregular',"
                + " parameter " + m_server.PortableCharacterType(100) + " not null default '',"
                + " tablename " + m_server.PortableCharacterType(128) + " not null default '',"
//                + " fileindex int not null default 0," // main file is 0, next 1, etc..
                + " provider " + m_server.PortableCharacterType(200) + " not null default '', "
                + " connectionstring " + m_server.PortableCharacterType(2600) + " not null default '', "
                + " expression " + m_server.PortableCharacterType(2048) + " not null default '', "
                + " notes " + m_server.PortableCharacterType(2048) + " not null default '', "
              //  + " acl " + m_server.PortableCharacterType(50)+ " not null default '',"
                + " enabled smallint not null default 1"
                + "  )";

            ExecuteCreateTable(m_server, sql);
        }

      

        private void CreateLimitTable()
        {
            if (m_server.TableExists("quality_limit"))
            {
                return;
            }
            string sql = "Create Table quality_limit "
                + " ( "
                + " tablemask " + m_server.PortableCharacterType(100) + " not null primary key,"
                + " High float null default null, "
                + " Low float null default null, "
                + " Change float null default null "
                + "  )";

            ExecuteCreateTable(m_server, sql);
        }
        //private void CreateCalculationTable()
        //{
        //    if (m_server.TableExists("calculation"))
        //    {
        //        return;
        //    }
        //    string sql = "Create Table calculation "
        //        + " ( "
        //        + " tablemask " + m_server.PortableCharacterType(100) + " not null primary key,"
        //        + " expression "+ m_server.PortableCharacterType(1024) +" not null default '' , "
        //        + " interval " + m_server.PortableCharacterType(20) + " not null default 'instant' , "
        //        + " group_name " + m_server.PortableCharacterType(20) + " not null default ''  "
        //        + "  )";

        //    ExecuteCreateTable(m_server, sql);
        //}








        
    }
}