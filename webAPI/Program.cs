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
                try
                {
                    //Прослушивание порта
                    TcpClient client = serverSocket.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();

                    BinaryReader reader = new BinaryReader(stream);
                    string message = reader.ReadString();
                    Console.WriteLine("Получено: " + message);
                    BinaryWriter writer = new BinaryWriter(stream);
                    //Отправка полученного запроса в СУБД
                    writer.Write(sql.sqlServer(message));
                    writer.Flush();
                    writer.Close();
                    reader.Close();
                    stream.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ошибка: " + ex.ToString());
                }
            }
        }

        private class sql
        {
            public static string ConString = @"Data Source=192.168.0.105,1433\sqlexpress;Initial Catalog=NevaLink;User ID=dmitryHome;Password=wpdtf1234";

            public static string sqlServer(string message)
            {
                string results = "";
                SqlConnection con = new SqlConnection(sql.ConString);
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
                    Console.WriteLine("Отправлено: запрос с данными");
                }
                else
                {
                    results = "No";
                    Console.WriteLine("Отправлено: пустой ответ");
                }
                read.Close();
                con.Close();
                return results;
            }
        }
    }
}
