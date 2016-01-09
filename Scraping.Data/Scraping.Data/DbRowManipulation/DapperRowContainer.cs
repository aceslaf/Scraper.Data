
using Scraper.Common;
using Scraper.Data.Common;
using Scraper.Data.RowManipulation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Scraper.Data.DbRowManipulation
{
    class DapperRowContainer : IRowContainer
    {
        private HashSet<string> columns;
        private string _tableName;
        private SqlConnection _connection;
        private DbCommandBuilder commandBuilder;

        public int RowCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DapperRowContainer(string tableName, SqlConnection connection)
        {
            _tableName = tableName;
            _connection = connection;
            columns = new HashSet<string>();
            DbProviderFactory factory = DbProviderFactories.GetFactory(_connection);
            commandBuilder = factory.CreateCommandBuilder();
            CreateTableIfNotExists();
            GetExistingColumns()
                       .ToList()
                       .ForEach(x => columns.Add(x));
        }


        private IEnumerable<string> GetExistingColumns()
        {
            return new List<string>();
        }

        private void CreateTableIfNotExists()
        {
            string pureTableName = commandBuilder.QuoteIdentifier(_tableName);
            string createTableComand = string.Format(@"if not exists (select * from sysobjects where name=@tableName and xtype='U')
                                                            create table {0} (
                                                                [hiddenIdentity] int
                                                            )",pureTableName);
            
            using (SqlCommand command = _connection.CreateCommand())
            {
                command.CommandText = createTableComand;
                command.Parameters.AddWithValue("@tableName", _tableName);
                ExecuteCmd(() => command.ExecuteNonQuery());
            }
        }

        public int AddRow(Row externalRow)
        {
            var row = externalRow.Columns.Select(x => new KeyValuePair<string, string>(x.Key.ToColumnName(), x.Value));
            foreach (var item in row)
            {
                GetOrCreateColumn(item.Key.ToColumnName());
            }

            //Execute insert statement

            //Row with properly escaped column names
            var pureRow = row.Select(x => new KeyValuePair<string, string>(commandBuilder.QuoteIdentifier(x.Key.ToColumnName()), x.Value)).ToList();

            var valueByKey = pureRow.ToDictionary(x => x.Key, x => x.Value);
            var parameterNamesByKey = new Dictionary<string, string>();
            int i = 1;
            foreach (var pair in pureRow)
            {
                parameterNamesByKey.AddOrOvewrite(pair.Key, string.Format("@a{0}", i));
                i++;
            }

            string pureTableName = commandBuilder.QuoteIdentifier(_tableName);

            var columNames = parameterNamesByKey.ToList()
                                                .Select(x => x.Key)
                                                .Aggregate((l, r) => string.Format("{0},{1}", l, r));

            var valueParameters = parameterNamesByKey.ToList()
                                                          .Select(x => x.Value)
                                                          .Aggregate((l, r) => string.Format("{0},{1}", l, r));

            var insertComand = string.Format(@"INSERT INTO {0} ({1})
                                 VALUES({2})",pureTableName, columNames,valueParameters);

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = insertComand;
                foreach (var pair in parameterNamesByKey)
                {
                    var value = valueByKey[pair.Key];
                    var paramName = pair.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        cmd.Parameters.AddWithValue(paramName, DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(paramName, value);
                    }
                }
                ExecuteCmd(() => cmd.ExecuteNonQuery());
            }
                //TODO return our new autoincremented default identity
                return 1;
        }

        public int GetOrCreateColumn(string externalCOlumnName)
        {
            var columnName = externalCOlumnName.ToColumnName();
            if (!columns.Contains(columnName))
            {
                string createTableComand = @"IF NOT EXISTS (
                                              SELECT * 
                                              FROM   sys.columns 
                                              WHERE  object_id = OBJECT_ID(@table_name) 
                                                     AND name = @columnName
                                            )
                                                ALTER TABLE {0}
                                                ADD {1} NVARCHAR(1000) NULL
                                            ";
                string pureTableName = commandBuilder.QuoteIdentifier(_tableName);
                string pureColumnName = commandBuilder.QuoteIdentifier(columnName);

                using (SqlCommand command = _connection.CreateCommand())
                {
                    command.CommandText = string.Format(createTableComand,pureTableName,pureColumnName);
                    command.Parameters.AddWithValue("@table_name", _tableName);
                    command.Parameters.AddWithValue("@columnName", columnName);
                    ExecuteCmd(() => command.ExecuteNonQuery());
                }
                columns.Add(columnName);
            }

            //TODO Decide if this should stay in the api since it is not used anymore
            return 1;
        }

        public Row GetRow(int i)
        {
            //get row by id
            throw new NotImplementedException();
        }

        private void ExecuteCmd(Action cmdExecution)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                cmdExecution();
            }
            catch (Exception e)
            {
                //Todo implement error handling
                    throw;
            }finally
            {
                _connection.Close();
            } 
        }
    }
}
