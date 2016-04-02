using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VanEmdeBoasTree
{
    public class RsVanEmdeBoasTree<TData> : IVanEmdeBoasTree<TData>
    {
        public uint Universe { get; }
        public uint? Min { get; protected set; }
        public TData MinData { get; protected set; }
        public uint? Max { get; protected set; }
        public TData MaxData { get; protected set; }
        public IVanEmdeBoasTree<TData> Summary { get; private set; }
        private readonly Dictionary<int, RsVanEmdeBoasTree<TData>> _clusters;
        private readonly uint _lowerSqrt;

        public RsVanEmdeBoasTree(uint universe)
        {
            if (universe <= 0)
                Universe = 2;
            else Universe = (uint)Math.Pow(2, Math.Max(1, Math.Ceiling(Math.Log(universe, 2))));

            if (universe > 2)
            {
                _lowerSqrt = (uint)Math.Pow(2, Math.Floor(Math.Log(Universe, 2) / 2));
                _clusters = new Dictionary<int, RsVanEmdeBoasTree<TData>>();
            }
        }

        public IEnumerable<IVanEmdeBoasTree<TData>> Clusters
            => _clusters?.Values ?? Enumerable.Empty<IVanEmdeBoasTree<TData>>();

        public void Insert(uint value, TData data)
        {
            if (value >= Universe)
                throw new ArgumentOutOfRangeException();
            DoInsert(value, data);
        }

        private void DoInsert(uint value, TData data)
        {
            if (!Min.HasValue)
            {
                Min = value;
                Max = value;
                MinData = data;
                MaxData = data;
            }
            else
            {
                if (value < Min)
                {
                    var temp = Min.Value;
                    Min = value;
                    value = temp;
                    var tempData = MinData;
                    MinData = data;
                    data = tempData;
                }
                if (Universe > 2)
                {
                    var clusterIdx = High(value);
                    var cluster = GetOrCreateCluster(clusterIdx);
                    if (cluster.Min.HasValue)
                    {
                        cluster.DoInsert(Low(value), data);
                    }
                    else
                    {
                        GetOrCreateSummary().Insert(clusterIdx, data);
                        cluster.Min = Low(value);
                        cluster.MinData = data;
                        cluster.Max = Low(value);
                        cluster.MaxData = data;
                    }
                }
                if (value > Max)
                {
                    Max = value;
                    MaxData = data;
                }
            }
        }

        public bool Contains(uint value)
        {
            if (value == Min || value == Max)
                return true;
            if (Universe <= 2)
                return false;
            var cluster = GetCluster(High(value));
            return cluster?.Contains(Low(value)) ?? false;
        }

        public uint? GetSuccessor(uint value)
        {
            if (Universe == 2)
            {
                if (Max.HasValue && Max > value)
                    return Max.Value;
                return null;
            }
            if (Min.HasValue && Min > value)
                return Min;
            var cluster = GetCluster(High(value));
            if (cluster?.Max != null && cluster.Max > Low(value))
            {
                var lowSuccessor = cluster.GetSuccessor(Low(value));
                Debug.Assert(lowSuccessor.HasValue);
                return Index(High(value), lowSuccessor.Value);
            }
            else
            {
                var nextClusterIdx = Summary?.GetSuccessor(High(value));
                if (!nextClusterIdx.HasValue)
                    return null;
                var nextCluster = GetCluster(nextClusterIdx.Value);
                Debug.Assert(nextCluster?.Min != null);
                return Index(nextClusterIdx.Value, nextCluster.Min.Value);
            }
        }

        public uint? GetPredecessor(uint value)
        {
            if (Universe == 2)
            {
                if (Min.HasValue && Min < value)
                    return Min.Value;
                return null;
            }
            if (Max.HasValue && Max < value)
                return Max;
            var cluster = GetCluster(High(value));
            if (cluster?.Min != null && cluster.Min < Low(value))
            {
                var lowPredecessor = cluster.GetPredecessor(Low(value));
                Debug.Assert(lowPredecessor.HasValue);
                return Index(High(value), lowPredecessor.Value);
            }
            else
            {
                var prevClusterIdx = Summary?.GetPredecessor(High(value));
                if (!prevClusterIdx.HasValue)
                {
                    if (Min.HasValue && Min < value)
                        return Min;
                    return null;
                }
                var prevCluster = GetCluster(prevClusterIdx.Value);
                Debug.Assert(prevCluster?.Max != null);
                return Index(prevClusterIdx.Value, prevCluster.Max.Value);
            }
        }

        public void Delete(uint value)
        {
            if (Min == Max)
            {
                Min = null;
                Max = null;
                MinData = default(TData);
                MaxData = default(TData);
                Summary = null;
            }
            else if (Universe == 2)
            {
                Debug.Assert(value == 0 || value == 1);
                if (value == 0)
                {
                    Min = 1;
                    MinData = MaxData;
                }
                else
                {
                    Max = 0;
                    MaxData = MinData;
                }
            }
            else
            {
                if (value == Min)
                {
                    var minClusterIdx = Summary?.Min;
                    Debug.Assert(minClusterIdx.HasValue);
                    var clusterWithMin = GetCluster(minClusterIdx.Value);
                    Debug.Assert(clusterWithMin != null);
                    var minClusterMin = clusterWithMin.Min;
                    Debug.Assert(minClusterMin.HasValue);
                    Min = Index(minClusterIdx.Value, minClusterMin.Value);
                    MinData = clusterWithMin.MinData;
                    value = Min.Value;
                }
                var clusterToDeleteFrom = GetCluster(High(value));
                if (clusterToDeleteFrom != null)
                {
                    clusterToDeleteFrom.Delete(Low(value));

                    if (clusterToDeleteFrom.Min == null)
                    {
                        _clusters.Remove((int)High(value));
                        Summary?.Delete(High(value));
                        if (value == Max)
                        {
                            var maxClusterIdx = Summary?.Max;
                            if (maxClusterIdx.HasValue)
                            {
                                var maxCluster = GetCluster(maxClusterIdx.Value);
                                Debug.Assert(maxCluster.Max.HasValue);
                                Max = Index(maxClusterIdx.Value, maxCluster.Max.Value);
                                MaxData = maxCluster.MaxData;
                            }
                            else
                            {
                                Max = Min;
                            }
                        }
                    }
                    else if (value == Max)
                    {
                        var newMax = clusterToDeleteFrom.Max;
                        Debug.Assert(newMax.HasValue);
                        Max = Index(High(value), newMax.Value);
                        MaxData = clusterToDeleteFrom.MaxData;
                    }
                }
            }
        }

        private RsVanEmdeBoasTree<TData> GetCluster(uint clusterIdx)
        {
            if (!_clusters.ContainsKey((int) clusterIdx))
                return null;
            return _clusters[(int) clusterIdx];
        }

        private RsVanEmdeBoasTree<TData> GetOrCreateCluster(uint clusterIdx)
        {
            if (!_clusters.ContainsKey((int)clusterIdx))
                _clusters[(int)clusterIdx] = new RsVanEmdeBoasTree<TData>(_lowerSqrt);
            return _clusters[(int)clusterIdx];
        }

        private IVanEmdeBoasTree<TData> GetOrCreateSummary()
        {
            if (Summary == null)
            {
                var upperSqrt = (uint)Math.Pow(2, Math.Ceiling(Math.Log(Universe, 2) / 2));
                Summary = new RsVanEmdeBoasTree<TData>(upperSqrt);
            }
            return Summary;
        }

        private uint Index(uint high, uint low)
        {
            return high * _lowerSqrt + low;
        }

        private uint High(uint value)
        {
            return value / _lowerSqrt;
        }

        private uint Low(uint value)
        {
            return value % _lowerSqrt;
        }
    }
}