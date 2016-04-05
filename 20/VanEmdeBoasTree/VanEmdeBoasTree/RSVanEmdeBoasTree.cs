using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VanEmdeBoasTree
{
    public class RsVanEmdeBoasTree<TData> : IVanEmdeBoasTree<TData>
    {
        public int Universe { get; }
        public int? Min { get; protected set; }
        public TData MinData { get; protected set; }
        public int? Max { get; protected set; }
        public TData MaxData { get; protected set; }
        public IVanEmdeBoasTree<TData> Summary { get; private set; }
        private readonly Dictionary<int, RsVanEmdeBoasTree<TData>> _clusters;
        private readonly int _lowerSqrt;

        public RsVanEmdeBoasTree(int universe)
        {
            if (universe <= 0)
                Universe = 2;
            else Universe = (int)Math.Pow(2, Math.Max(1, Math.Ceiling(Math.Log(universe, 2))));

            if (universe > 2)
            {
                _lowerSqrt = (int)Math.Pow(2, Math.Floor(Math.Log(Universe, 2) / 2));
                _clusters = new Dictionary<int, RsVanEmdeBoasTree<TData>>();
            }
        }

        public IEnumerable<IVanEmdeBoasTree<TData>> Clusters
            => _clusters?.Values ?? Enumerable.Empty<IVanEmdeBoasTree<TData>>();

        public void Insert(int key, TData data)
        {
            if (key >= Universe)
                throw new ArgumentOutOfRangeException();
            DoInsert(key, data);
        }

        private void DoInsert(int value, TData data)
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

        public bool Contains(int value)
        {
            if (value == Min || value == Max)
                return true;
            if (Universe <= 2)
                return false;
            var cluster = GetCluster(High(value));
            return cluster?.Contains(Low(value)) ?? false;
        }

        public int? GetSuccessor(int key)
        {
            if (Universe == 2)
            {
                if (Max.HasValue && Max > key)
                    return Max.Value;
                return null;
            }
            if (Min.HasValue && Min > key)
                return Min;
            var cluster = GetCluster(High(key));
            if (cluster?.Max != null && cluster.Max > Low(key))
            {
                var lowSuccessor = cluster.GetSuccessor(Low(key));
                Debug.Assert(lowSuccessor.HasValue);
                return Index(High(key), lowSuccessor.Value);
            }
            else
            {
                var nextClusterIdx = Summary?.GetSuccessor(High(key));
                if (!nextClusterIdx.HasValue)
                    return null;
                var nextCluster = GetCluster(nextClusterIdx.Value);
                Debug.Assert(nextCluster?.Min != null);
                return Index(nextClusterIdx.Value, nextCluster.Min.Value);
            }
        }

        public int? GetPredecessor(int key)
        {
            if (Universe == 2)
            {
                if (Min.HasValue && Min < key)
                    return Min.Value;
                return null;
            }
            if (Max.HasValue && Max < key)
                return Max;
            var cluster = GetCluster(High(key));
            if (cluster?.Min != null && cluster.Min < Low(key))
            {
                var lowPredecessor = cluster.GetPredecessor(Low(key));
                Debug.Assert(lowPredecessor.HasValue);
                return Index(High(key), lowPredecessor.Value);
            }
            else
            {
                var prevClusterIdx = Summary?.GetPredecessor(High(key));
                if (!prevClusterIdx.HasValue)
                {
                    if (Min.HasValue && Min < key)
                        return Min;
                    return null;
                }
                var prevCluster = GetCluster(prevClusterIdx.Value);
                Debug.Assert(prevCluster?.Max != null);
                return Index(prevClusterIdx.Value, prevCluster.Max.Value);
            }
        }

        public void Delete(int key)
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
                Debug.Assert(key == 0 || key == 1);
                if (key == 0)
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
                if (key == Min)
                {
                    var minClusterIdx = Summary?.Min;
                    Debug.Assert(minClusterIdx.HasValue);
                    var clusterWithMin = GetCluster(minClusterIdx.Value);
                    Debug.Assert(clusterWithMin != null);
                    var minClusterMin = clusterWithMin.Min;
                    Debug.Assert(minClusterMin.HasValue);
                    Min = Index(minClusterIdx.Value, minClusterMin.Value);
                    MinData = clusterWithMin.MinData;
                    key = Min.Value;
                }
                var clusterToDeleteFrom = GetCluster(High(key));
                if (clusterToDeleteFrom != null)
                {
                    clusterToDeleteFrom.Delete(Low(key));

                    if (clusterToDeleteFrom.Min == null)
                    {
                        _clusters.Remove(High(key));
                        Summary?.Delete(High(key));
                        if (key == Max)
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
                    else if (key == Max)
                    {
                        var newMax = clusterToDeleteFrom.Max;
                        Debug.Assert(newMax.HasValue);
                        Max = Index(High(key), newMax.Value);
                        MaxData = clusterToDeleteFrom.MaxData;
                    }
                }
            }
        }

        private RsVanEmdeBoasTree<TData> GetCluster(int clusterIdx)
        {
            if (!_clusters.ContainsKey(clusterIdx))
                return null;
            return _clusters[clusterIdx];
        }

        private RsVanEmdeBoasTree<TData> GetOrCreateCluster(int clusterIdx)
        {
            if (!_clusters.ContainsKey(clusterIdx))
                _clusters[clusterIdx] = new RsVanEmdeBoasTree<TData>(_lowerSqrt);
            return _clusters[clusterIdx];
        }

        private IVanEmdeBoasTree<TData> GetOrCreateSummary()
        {
            if (Summary == null)
            {
                var upperSqrt = (int)Math.Pow(2, Math.Ceiling(Math.Log(Universe, 2) / 2));
                Summary = new RsVanEmdeBoasTree<TData>(upperSqrt);
            }
            return Summary;
        }

        private int Index(int high, int low)
        {
            return high * _lowerSqrt + low;
        }

        private int High(int value)
        {
            return value / _lowerSqrt;
        }

        private int Low(int value)
        {
            return value % _lowerSqrt;
        }
    }
}