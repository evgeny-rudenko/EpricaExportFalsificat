using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

namespace EpricaExportFalsificat
{
    class Program
    {
        static void Main(string[] args)
        {
            string cs = "Data Source=" + Properties.Settings.Default.EpricaDB + " ;PRAGMA encoding = '1251';";
            var con = new SQLiteConnection(cs);
            con.Open();

            string sqlqry = Resource.ReadResource("Falsificat.SQL");
            
            var res = con.Query(sqlqry);
            DataTable dt = new DataTable();

            dt.Columns.Add("GOOD_NAME");
            dt.Columns.Add("PRODUCER_NAME");
            dt.Columns.Add("COUNTRY");
            dt.Columns.Add("SERIA");
            dt.Columns.Add("REASON");
            dt.Columns.Add("DOCNUM");
            dt.Columns.Add("DOCDATE");
            dt.Columns.Add("LAB_NAME");

            Console.WriteLine("Читаю данные из Eprica");
            using (var reader = con.ExecuteReader(sqlqry))
            {
                
                Console.WriteLine("Записываю данные из Eprica в память");
                while (reader.Read())
                {
                
                    DataRow row = dt.NewRow();
                    row["GOOD_NAME"] = decodehex(reader.GetString(reader.GetOrdinal("GOOD_NAME")));
                    row["PRODUCER_NAME"] = decodehex(reader.GetString(reader.GetOrdinal("PRODUCER_NAME")));
                    row["COUNTRY"] = decodehex(reader.GetString(reader.GetOrdinal("COUNTRY")));
                    row["SERIA"] = decodehex(reader.GetString(reader.GetOrdinal("SERIA")));
                    row["REASON"] = decodehex(reader.GetString(reader.GetOrdinal("REASON")));
                    row["LAB_NAME"] = decodehex(reader.GetString(reader.GetOrdinal("LAB_NAME")));
                    row["DOCNUM"] = decodehex(reader.GetString(reader.GetOrdinal("DOCNUM")));
                    row["DOCDATE"] = UnixTimeStampToDateTime( double.Parse( decodehex( reader.GetString(reader.GetOrdinal("DOCDATE")))));
                    
                    dt.Rows.Add(row);

                    /*
                    Console.WriteLine( decodehex ( reader.GetString(reader.GetOrdinal("GOOD_NAME"))));
                    Console.WriteLine(decodehex(reader.GetString(reader.GetOrdinal("DOCNUM"))));
                    Console.WriteLine(UnixTimeStampToDateTime(double.Parse(decodehex(reader.GetString(reader.GetOrdinal("DOCDATE"))))));
                    */
                }
            }

            Console.WriteLine("Записываю данные о фальсификатах в файл");
            DbfWriterFast.Write(dt, Properties.Settings.Default.SaveTo, "falsificat");
            
            Console.WriteLine("Готово. Закрываем программу");

        }


        /// <summary>
        /// Функция преобразует Unix время в DateTime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        /// <summary>
        /// Декодируем строку HEX в текст
        /// </summary>
        /// <param name="hexstring"></param>
        /// <returns></returns>
        static string decodehex (string hexstring)
        {
            byte[] data = FromHex(hexstring);
            string s = Encoding.GetEncoding(1251).GetString(data); 

            return s;
        }

        /// <summary>
        /// Преобразуем шестнадцатеричную строку в стандартное значение String
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
