using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Data.SqlClient;

namespace webAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Определение доступа к сети и выбор порта
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
            Console.WriteLine("Server start; ip = "+ IPAddress.Any.ToString());
            serverSocket.Start();

            while (true)
            {
                //Прослушивание порта
                TcpClient client = serverSocket.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                BinaryReader reader = new BinaryReader(stream);
                string message = reader.ReadString();
                Console.WriteLine("Получено: " + message);
                BinaryWriter writer = new BinaryWriter(stream);

                string results = "";
                //Отправка полученного запроса в СУБД
                SqlConnection con = new SqlConnection(sqlCon.ConString);
                con.Open();
                SqlCommand com = new SqlCommand(message, con);
                SqlDataReader read = com.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string Field = "";
                        int CountField = read.FieldCount;
                        for (int i = 0; i < CountField; i++)
                        {
                            Field = Field + "|" + read.GetValue(i).ToString();
                        }
                        results = results + "^" + Field;

                    }
                    Console.WriteLine("Отправлено: " + results);
                    writer.Write(results);

                }
                else
                {
                    results = "No";
                    Console.WriteLine("Отправлено: " + results);
                    writer.Write(results);
                }


                read.Close();
                con.Close();


                writer.Flush();

                writer.Close();
                reader.Close();
                stream.Close();
                client.Close();

            }
        }

        public class sqlCon
        {
            public static string ConString = @"Data Source=192.168.0.105,1433\sqlexpress;Initial Catalog=NevaLink;User ID=dmitryHome;Password=wpdtf1234";

        }
    }
}
