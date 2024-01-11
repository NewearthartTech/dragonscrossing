using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DragonsCrossing.Core;

public interface IDbServiceBase
{
    IMongoDatabase? db { get; }
    IMongoCollection<T> getCollection<T>();
    Task<IClientSessionHandle> StartTransaction();
}

public interface ISeasonsDbService : IDbServiceBase
{
    bool isAvailable { get; }
    int seasonId { get; }
};

public interface IPerpetualDbService : IDbServiceBase { };

public class MongoConfig
{
    public string connectionString { get; set; } = "mongodb://localhost?connect=direct";
    public string dbNamePrefix { get; set; } = "dcx";
}

public class NoSeasonsAvailable : ISeasonsDbService
{
    public bool isAvailable => false;
    public int seasonId => -1;
    public IMongoDatabase? db => null;
    public IMongoCollection<T> getCollection<T>() { throw new NotImplementedException(); }
    public Task<IClientSessionHandle> StartTransaction() { throw new NotImplementedException(); }
}

public class SeasonsDbService : DbServiceBase, ISeasonsDbService
{
    readonly int _seasonId;
    public SeasonsDbService(IConfiguration configuration, ILogger<SeasonsDbService> logger, int seasonId)
        :base(configuration, logger,c=>$"{c.dbNamePrefix}_season_{seasonId}")
    {
        if (0 == seasonId)
            throw new Exception("empty season id");
        _seasonId = seasonId;
    }

    public bool isAvailable => true;
    public int seasonId => _seasonId;
}



public class PerpetualDbService : DbServiceBase, IPerpetualDbService
{
    public PerpetualDbService(IConfiguration configuration, ILogger<SeasonsDbService> logger)
        : base(configuration, logger, c => $"{c.dbNamePrefix}_perpetual")
    {
    }
}



public abstract class DbServiceBase 
{
    readonly MongoClient _client;
    readonly IMongoDatabase _db;
    readonly ILogger _logger;

    public DbServiceBase(IConfiguration configuration, ILogger logger, Func<MongoConfig, string> getDatabaseName)
    {
        _logger = logger;
        var config = configuration.GetSection("mongo").Get<MongoConfig>() ?? new MongoConfig();

        var databaseName = getDatabaseName(config);

        _logger.LogTrace($"using mongo connection ${config.connectionString} -> ${databaseName}");

        _client = new MongoClient(config.connectionString);
        _db = _client.GetDatabase(databaseName);
    }

    static DbServiceBase()
    {
        var pack = new ConventionPack();
        pack.Add(new IgnoreExtraElementsConvention(true));
        ConventionRegistry.Register("Custom Conventions", pack, t => true);
    }

    public IMongoDatabase db => _db;


    public async Task<IClientSessionHandle> StartTransaction()
    {
        var ret =  await _client.StartSessionAsync();
        ret.StartTransaction();

        return ret;
    }

    public IMongoCollection<T> getCollection<T>()
    {
        var attribute = typeof(T).GetCustomAttributes<MongoCollectionAttribute>(true).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(attribute?.collectionName))
        {
            Debug.Assert(false);
            throw new Exception("MongoCollection not defined");
        }

        var collection = db.GetCollection<T>(attribute.collectionName);

        CreateIndexes<T>(collection);

        return collection;
    }

    /// <summary>
    /// Used to create the Index the first time we create the db
    /// </summary>
    static ConcurrentDictionary<Type, bool> _indexesCreated = new ConcurrentDictionary<Type, bool>();

    void CreateIndexes<T>(IMongoCollection<T> collection)
    {
        var theType = typeof(T);

        if (_indexesCreated.ContainsKey(theType))
            return;

        var allStatics = theType.GetMethods(BindingFlags.Static | BindingFlags.Public);

        var allDone = allStatics
            .Where(m => m.GetCustomAttributes<MongoIndexAttribute>(false).Count() > 0)
            .Select(m =>
            {
                m.Invoke(null, new[] { collection });
                return true;
            })
            .ToArray();
        ;

        /*
        chatItems.ChatMessageModel.CreateIndexes(_db);
        login.UserModel.CreateIndexes(_db);
        */

        _indexesCreated[theType] = true;
    }
}

/// <summary>
/// defines what collection this Class is stored in
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class MongoCollectionAttribute : Attribute
{
    public string collectionName { get; private set; }
    public MongoCollectionAttribute(string collectionName)
    {
        this.collectionName = collectionName;
    }
}

/// <summary>
/// This methods will be called to Create Indexes on the object
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class MongoIndexAttribute : Attribute { }


