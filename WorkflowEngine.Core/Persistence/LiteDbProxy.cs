using LiteDB;


namespace WorkflowEngine.Core.Persistence
{
    public enum DbOperationType
    {
        Insert,
        Update,
        Delete
    }

    public class DbOperation
    {
        public DbOperationType Type { get; set; }
        public object Entity { get; set; }
        public Type EntityType { get; set; }
    }

   

public class LiteDbContext : IDisposable
    {
        private readonly LiteDatabase _database;
        private readonly List<DbOperation> _operations = new();

        public LiteDbContext(string connectionString)
        {
            _database = new LiteDatabase(connectionString);
        }

        public ILiteCollection<T> Set<T>()
        {
            return _database.GetCollection<T>();
        }


        public void Add<T>(T entity)
        {
            _operations.Add(new DbOperation
            {
                Type = DbOperationType.Insert,
                Entity = entity!,
                EntityType = typeof(T)
            });
        }

        public void Update<T>(T entity)
        {
            _operations.Add(new DbOperation
            {
                Type = DbOperationType.Update,
                Entity = entity!,
                EntityType = typeof(T)
            });
        }

        public void Delete<T>(T entity)
        {
            _operations.Add(new DbOperation
            {
                Type = DbOperationType.Delete,
                Entity = entity!,
                EntityType = typeof(T)
            });
        }

        public void SaveChanges()
        {
            if (_operations.Count == 0)
                return;

            _database.BeginTrans();
            try
            {
                foreach (var op in _operations)
                {
                    var collection = _database.GetCollection(op.Entity.GetType().Name.ToLower());
                    var doc = BsonMapper.Global.ToDocument(op.Entity);
                    switch (op.Type)
                    {
                        case DbOperationType.Insert:
                            collection.Insert(doc);
                            break;

                        case DbOperationType.Update:
                            collection.Update(doc);
                            break;

                        case DbOperationType.Delete:
                            var idProp = op.Entity.GetType().GetProperty("Id");
                            if (idProp == null) throw new Exception("Entity must have Id property");
                            var idValue = idProp.GetValue(doc);
                            collection.Delete(new BsonValue(idValue));
                            break;
                    }
                }

                _database.Commit();
                _operations.Clear();
            }
            catch(Exception ex)
            {
                _database.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }
}
