﻿using System;
using System.Collections.Generic;
using NRedisTimeSeries.DataTypes;
using StackExchange.Redis;
using Xunit;

namespace NRedisTimeSeries.Test.TestAPI
{
    public class TestIncrBy : AbstractTimeSeriesTest, IDisposable
    {
        private readonly string key = "INCRBY_TESTS";

        public TestIncrBy(RedisFixture redisFixture) : base(redisFixture) { }

        public void Dispose()
        {
            redisFixture.Redis.GetDatabase().KeyDelete(key);
        }

        [Fact]
        public void TestDefualtIncrBy()
        {
            double value = 5.5;
            IDatabase db = redisFixture.Redis.GetDatabase();
            Assert.True(db.TimeSeriesIncerBy(key, value) > 0);
            Assert.Equal(value, db.TimeSeriesGet(key).Val);
        }

        [Fact]
        public void TestStarIncrBy()
        {
            double value = 5.5;
            IDatabase db = redisFixture.Redis.GetDatabase();
            Assert.True(db.TimeSeriesIncerBy(key, value, timestamp: "*") > 0);
            Assert.Equal(value, db.TimeSeriesGet(key).Val);
        }

        [Fact]
        public void TestIncrByTimeStamp()
        {
            double value = 5.5;
            IDatabase db = redisFixture.Redis.GetDatabase();
            TimeStamp timeStamp = DateTime.Now;
            Assert.Equal(timeStamp, db.TimeSeriesIncerBy(key, value, timestamp: timeStamp));
            Assert.Equal(new TimeSeriesTuple(timeStamp, value), db.TimeSeriesGet(key));
        }

        [Fact]
        public void TestDefualtIncrByWithRetentionTime()
        {
            double value = 5.5;
            long retentionTime = 5000;
            IDatabase db = redisFixture.Redis.GetDatabase();
            Assert.True(db.TimeSeriesIncerBy(key, value, retentionTime: retentionTime) > 0);
            Assert.Equal(value, db.TimeSeriesGet(key).Val);
            TimeSeriesInformation info = db.TimeSeriesInfo(key);
            Assert.Equal(retentionTime, info.RetentionTime);
        }

        [Fact]
        public void TestDefaultIncrByWithLabels()
        {
            double value = 5.5;
            TimeSeriesLabel label = new TimeSeriesLabel("key", "value");
            IDatabase db = redisFixture.Redis.GetDatabase();
            var labels = new List<TimeSeriesLabel> { label };
            Assert.True(db.TimeSeriesIncerBy(key, value, labels: labels) > 0);
            Assert.Equal(value, db.TimeSeriesGet(key).Val);
            TimeSeriesInformation info = db.TimeSeriesInfo(key);
            Assert.Equal(labels, info.Labels);
        }

        [Fact]
        public void TestDefualtIncrByWithUncompressed()
        {
            double value = 5.5;
            IDatabase db = redisFixture.Redis.GetDatabase();
            Assert.True(db.TimeSeriesIncerBy(key, value, uncompressed:true) > 0);
            Assert.Equal(value, db.TimeSeriesGet(key).Val);
        }

        [Fact]
        public void TestWrongParameters()
        {
            double value = 5.5;
            IDatabase db = redisFixture.Redis.GetDatabase();
            var ex = Assert.Throws<RedisServerException>(() => db.TimeSeriesIncerBy(key, value, timestamp: "+"));
            Assert.Equal("TSDB: invalid timestamp", ex.Message);
            ex = Assert.Throws<RedisServerException>(() => db.TimeSeriesIncerBy(key, value, timestamp: "-"));
            Assert.Equal("TSDB: invalid timestamp", ex.Message);
        }
    }
}