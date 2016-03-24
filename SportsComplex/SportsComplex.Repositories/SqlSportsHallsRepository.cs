using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlSportsHallsRepository : SqlBaseRepository, ISportsHallsRepository
    {
        public SqlSportsHallsRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IEnumerable<SportsHall> GetFreeSportsHalls()
        {
            var halls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM vFreeSportsHalls";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"],
                                Status = (int)reader["Status"]
                            };

                            halls.Add(sportsHall);
                        }
                    }
                }
            }

            return halls;
        }

        public IEnumerable<SportsHall> GetFreeSportsHallsByFilter(SportsHallType sportsHallType, int minArea, int maxArea, decimal minRate, decimal maxRate)
        {
            var sportsHalls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = getFreeHallsByFilter;

                    command.Parameters.AddWithValue("@areaMin", minArea);
                    command.Parameters.AddWithValue("@areaMax", maxArea);
                    command.Parameters.AddWithValue("@rateMin", minRate);
                    command.Parameters.AddWithValue("@rateMax", maxRate);
// maby, it will be usefully to create stored procedur? 
                    if (sportsHallType != null)
                    {
                        command.Parameters.AddWithValue("@hallTypeId", sportsHallType.Id);
                        command.CommandText += "AND c.ClassTypeId = @hallTypeId";
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"]
                            };
                            sportsHalls.Add(sportsHall);
                        }
                    }
                }
            }

            return sportsHalls;
        }

        public IEnumerable<SportsHall> SelectAll()
        {
            var sportsHalls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM vPriceList";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"]
                            };
                            sportsHalls.Add(sportsHall);
                        }
                    }
                }
            }

            return sportsHalls;
        }
// Review - Olya: you should declare class fields before method. 
        private const string getFreeHallsByFilter = @"SELECT c.Id, c.ClassTypeId as [TypeId], ct.Name as [TypeName], c.Area,                                            c.Rate, c.[Status]
	                                                FROM [tblClass] c
	                                                INNER JOIN [tblClassType] ct  ON c.ClassTypeId = ct.Id
	                                                WHERE c.[Status] = 0 
	                                                AND (c.Area BETWEEN @areaMin AND @areaMax)
	                                                AND (c.Rate BETWEEN @rateMin AND @rateMax)";
    }
}
