using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MokkivarausApp.Models;

namespace MokkivarausApp.Services
{
    public class DataService
    {
        // Adjust connection string to your setup
        private readonly string _connectionString =
            "server=127.0.0.1;port=3307;database=vn;user=root;password=Ruutti;";

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        // --------------------------------------------------------
        //  Asiakas
        // --------------------------------------------------------

        public async Task<Asiakas> GetOrCreateAsiakasByNameAsync(string fullName)
        {
            string[] parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string etunimi = parts.Length > 0 ? parts[0] : fullName;
            string sukunimi = parts.Length > 1 ? parts[1] : string.Empty;

            using var conn = CreateConnection();
            await conn.OpenAsync();

            // ensure dummy postal code exists
            using (var ensurePosti = conn.CreateCommand())
            {
                ensurePosti.CommandText =
                    "INSERT IGNORE INTO posti(postinro, toimipaikka) " +
                    "VALUES('00000', 'Tuntematon')";
                await ensurePosti.ExecuteNonQueryAsync();
            }

            // try to find existing
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT asiakas_id, postinro, etunimi, sukunimi, lahiosoite, email, puhelinnro " +
                    "FROM asiakas " +
                    "WHERE etunimi = @etu AND sukunimi = @suku LIMIT 1";
                cmd.Parameters.AddWithValue("@etu", etunimi);
                cmd.Parameters.AddWithValue("@suku", sukunimi);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Asiakas
                    {
                        AsiakasId = Convert.ToUInt32(reader["asiakas_id"]),
                        Postinro = reader["postinro"] as string,
                        Etunimi = reader["etunimi"] as string,
                        Sukunimi = reader["sukunimi"] as string,
                        Lahiosoite = reader["lahiosoite"] as string,
                        Email = reader["email"] as string,
                        Puhelinnro = reader["puhelinnro"] as string
                    };
                }
            }

            // create new
            using (var insert = conn.CreateCommand())
            {
                insert.CommandText =
                    "INSERT INTO asiakas(postinro, etunimi, sukunimi) " +
                    "VALUES('00000', @etu, @suku)";
                insert.Parameters.AddWithValue("@etu", etunimi);
                insert.Parameters.AddWithValue("@suku", sukunimi);
                await insert.ExecuteNonQueryAsync();
            }

            using (var idCmd = conn.CreateCommand())
            {
                idCmd.CommandText = "SELECT LAST_INSERT_ID()";
                var id = Convert.ToUInt32(await idCmd.ExecuteScalarAsync());

                return new Asiakas
                {
                    AsiakasId = id,
                    Postinro = "00000",
                    Etunimi = etunimi,
                    Sukunimi = sukunimi
                };
            }
        }

        // --------------------------------------------------------
        //  Mokki
        // --------------------------------------------------------

        public async Task<List<Mokki>> GetAllMokitAsync()
        {
            var list = new List<Mokki>();

            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT mokki_id, alue_id, postinro, mokkinimi, katuosoite, " +
                "       hinta, kuvaus, henkilomaara, varustelu " +
                "FROM mokki";

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                var mokki = new Mokki
                {
                    MokkiId = Convert.ToUInt32(r["mokki_id"]),
                    AlueId = Convert.ToUInt32(r["alue_id"]),
                    Postinro = r["postinro"] as string,
                    Mokkinimi = r["mokkinimi"] as string,
                    Katuosoite = r["katuosoite"] as string,
                    Hinta = Convert.ToDouble(r["hinta"]),
                    Kuvaus = r["kuvaus"] as string,
                    Varustelu = r["varustelu"] as string
                };

                if (!Convert.IsDBNull(r["henkilomaara"]))
                    mokki.Henkilomaara = Convert.ToInt32(r["henkilomaara"]);

                list.Add(mokki);
            }

            return list;
        }

        public async Task<uint> CreateMokkiAsync(Mokki m)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            // ensure postal code exists
            using (var ensurePosti = conn.CreateCommand())
            {
                ensurePosti.CommandText =
                    "INSERT IGNORE INTO posti(postinro, toimipaikka) " +
                    "VALUES(@postinro, '')";
                ensurePosti.Parameters.AddWithValue("@postinro", m.Postinro);
                await ensurePosti.ExecuteNonQueryAsync();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO mokki(alue_id, postinro, mokkinimi, katuosoite, hinta, kuvaus, henkilomaara, varustelu) " +
                    "VALUES(@alue, @postinro, @nimi, @katu, @hinta, @kuvaus, @hmaara, @varustelu)";
                cmd.Parameters.AddWithValue("@alue", m.AlueId);
                cmd.Parameters.AddWithValue("@postinro", m.Postinro);
                cmd.Parameters.AddWithValue("@nimi", m.Mokkinimi);
                cmd.Parameters.AddWithValue("@katu", m.Katuosoite);
                cmd.Parameters.AddWithValue("@hinta", m.Hinta);
                cmd.Parameters.AddWithValue("@kuvaus", m.Kuvaus);
                cmd.Parameters.AddWithValue("@hmaara", (object?)m.Henkilomaara ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@varustelu", m.Varustelu);
                await cmd.ExecuteNonQueryAsync();
            }

            using var idCmd = conn.CreateCommand();
            idCmd.CommandText = "SELECT LAST_INSERT_ID()";
            return Convert.ToUInt32(await idCmd.ExecuteScalarAsync());
        }

        public async Task UpdateMokkiAsync(Mokki m)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            // ensure postal code exists
            using (var ensurePosti = conn.CreateCommand())
            {
                ensurePosti.CommandText =
                    "INSERT IGNORE INTO posti(postinro, toimipaikka) " +
                    "VALUES(@postinro, '')";
                ensurePosti.Parameters.AddWithValue("@postinro", m.Postinro);
                await ensurePosti.ExecuteNonQueryAsync();
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "UPDATE mokki SET alue_id=@alue, postinro=@postinro, mokkinimi=@nimi, " +
                "katuosoite=@katu, hinta=@hinta, kuvaus=@kuvaus, henkilomaara=@hmaara, varustelu=@varustelu " +
                "WHERE mokki_id=@id";
            cmd.Parameters.AddWithValue("@alue", m.AlueId);
            cmd.Parameters.AddWithValue("@postinro", m.Postinro);
            cmd.Parameters.AddWithValue("@nimi", m.Mokkinimi);
            cmd.Parameters.AddWithValue("@katu", m.Katuosoite);
            cmd.Parameters.AddWithValue("@hinta", m.Hinta);
            cmd.Parameters.AddWithValue("@kuvaus", m.Kuvaus);
            cmd.Parameters.AddWithValue("@hmaara", (object?)m.Henkilomaara ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@varustelu", m.Varustelu);
            cmd.Parameters.AddWithValue("@id", m.MokkiId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteMokkiAsync(uint mokkiId)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM mokki WHERE mokki_id=@id";
            cmd.Parameters.AddWithValue("@id", mokkiId);
            await cmd.ExecuteNonQueryAsync();
        }

        // --------------------------------------------------------
        //  Varaus
        // --------------------------------------------------------

        public async Task<List<Varaus>> GetReservationsForCustomerAsync(uint asiakasId)
        {
            var list = new List<Varaus>();

            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"SELECT v.*, m.mokkinimi,
                         CONCAT(a.etunimi, ' ', a.sukunimi) AS asiakasNimi
                  FROM varaus v
                  JOIN mokki m   ON v.mokki_id = m.mokki_id
                  JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                  WHERE v.asiakas_id = @id
                  ORDER BY v.varattu_alkupvm";
            cmd.Parameters.AddWithValue("@id", asiakasId);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(MapVaraus(r));
            }

            return list;
        }

        public async Task<uint> CreateReservationAsync(
            uint asiakasId, uint mokkiId, DateTime alku, DateTime loppu)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO varaus
                        (asiakas_id, mokki_id, varattu_pvm, vahvistus_pvm,
                         varattu_alkupvm, varattu_loppupvm)
                      VALUES(@asiakas, @mokki, NOW(), NOW(), @alku, @loppu)";
                cmd.Parameters.AddWithValue("@asiakas", asiakasId);
                cmd.Parameters.AddWithValue("@mokki", mokkiId);
                cmd.Parameters.AddWithValue("@alku", alku);
                cmd.Parameters.AddWithValue("@loppu", loppu);
                await cmd.ExecuteNonQueryAsync();
            }

            using var idCmd = conn.CreateCommand();
            idCmd.CommandText = "SELECT LAST_INSERT_ID()";
            return Convert.ToUInt32(await idCmd.ExecuteScalarAsync());
        }

        public async Task UpdateReservationAsync(
            uint varausId, uint mokkiId, DateTime alku, DateTime loppu)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"UPDATE varaus
                  SET mokki_id=@mokki,
                      varattu_alkupvm=@alku,
                      varattu_loppupvm=@loppu
                  WHERE varaus_id=@id";
            cmd.Parameters.AddWithValue("@mokki", mokkiId);
            cmd.Parameters.AddWithValue("@alku", alku);
            cmd.Parameters.AddWithValue("@loppu", loppu);
            cmd.Parameters.AddWithValue("@id", varausId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteReservationAsync(uint varausId)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM varaus WHERE varaus_id=@id";
            cmd.Parameters.AddWithValue("@id", varausId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Varaus>> GetReservationsForMonthAsync(int year, int month)
        {
            var list = new List<Varaus>();
            DateTime start = new DateTime(year, month, 1);
            DateTime end = start.AddMonths(1);

            using var conn = CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"SELECT v.*, m.mokkinimi,
                         CONCAT(a.etunimi, ' ', a.sukunimi) AS asiakasNimi
                  FROM varaus v
                  JOIN mokki m   ON v.mokki_id = m.mokki_id
                  JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                  WHERE v.varattu_alkupvm < @end
                    AND v.varattu_loppupvm >= @start
                  ORDER BY v.varattu_alkupvm";
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(MapVaraus(r));
            }

            return list;
        }

        // --------------------------------------------------------
        //  Helpers
        // --------------------------------------------------------

        private static DateTime? GetNullableDate(DbDataReader r, string column)
        {
            var value = r[column];
            if (value == DBNull.Value) return null;
            return Convert.ToDateTime(value);
        }

        private Varaus MapVaraus(DbDataReader r)
        {
            return new Varaus
            {
                VarausId = Convert.ToUInt32(r["varaus_id"]),
                AsiakasId = Convert.ToUInt32(r["asiakas_id"]),
                MokkiId = Convert.ToUInt32(r["mokki_id"]),
                VarattuPvm = GetNullableDate(r, "varattu_pvm"),
                VahvistusPvm = GetNullableDate(r, "vahvistus_pvm"),
                VarattuAlkuPvm = GetNullableDate(r, "varattu_alkupvm"),
                VarattuLoppuPvm = GetNullableDate(r, "varattu_loppupvm"),
                MokkiNimi = r["mokkinimi"] as string,
                AsiakasNimi = r["asiakasNimi"] as string
            };
        }
    }
}
