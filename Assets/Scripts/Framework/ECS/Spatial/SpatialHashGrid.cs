using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Core
{
    /// <summary>
    /// 2D 空间哈希网格 —— 可复用基础设施。
    /// 用于将 O(N²) 邻居查询降至 O(N×k)。
    ///
    /// 使用方式（每帧）：
    ///   1. Clear()
    ///   2. Insert(entityId, x, y) × N
    ///   3. QueryNeighbors(x, y, radius, resultBuffer) → count
    ///
    /// 零热路径 GC：QueryNeighbors 结果写入调用方提供的 int[]。
    /// </summary>
    public class SpatialHashGrid
    {
        // cell key = (cellX << 32) | (uint)cellY，避免 string 装箱
        private readonly Dictionary<long, List<int>> _cells = new Dictionary<long, List<int>>();

        // 空闲 List<int> 对象池，避免 Clear 时销毁容器
        private readonly Stack<List<int>> _listPool = new Stack<List<int>>();

        // 已使用 key 列表，供 Clear 时快速回收（避免 Dictionary.Values 遍历装箱）
        private readonly List<long> _usedKeys = new List<long>(256);

        private readonly float _cellSize;
        private readonly float _invCellSize;

        public SpatialHashGrid(float cellSize)
        {
            _cellSize = cellSize;
            _invCellSize = 1f / cellSize;
        }

        // ------------------------------------------------
        // 每帧调用：清空内容，容器对象回收至对象池
        // ------------------------------------------------
        public void Clear()
        {
            for (int i = 0; i < _usedKeys.Count; i++)
            {
                if (_cells.TryGetValue(_usedKeys[i], out var list))
                {
                    list.Clear();
                    _listPool.Push(list);
                }
            }
            _cells.Clear();
            _usedKeys.Clear();
        }

        // ------------------------------------------------
        // 插入实体
        // ------------------------------------------------
        public void Insert(int entityId, float x, float y)
        {
            long key = MakeKey(CellX(x), CellY(y));

            if (!_cells.TryGetValue(key, out var list))
            {
                list = _listPool.Count > 0 ? _listPool.Pop() : new List<int>(8);
                _cells[key] = list;
                _usedKeys.Add(key);
            }

            list.Add(entityId);
                }

        // ------------------------------------------------
        // 查询邻居（零分配）
        // results 由调用方预分配；返回实际写入数量
        // 遍历以 (x,y) 为中心的 3×3 个 cell
        // ------------------------------------------------
public int QueryNeighbors(float x, float y, float radius, int[] results)
        {
            int cx = CellX(x);
            int cy = CellY(y);

            // 根据 radius 计算需要检查的 cell 范围
            int cellRadius = (int)(radius * _invCellSize) + 1;

            int count = 0;
            int maxResults = results.Length;

            for (int dx = -cellRadius; dx <= cellRadius; dx++)
            {
                for (int dy = -cellRadius; dy <= cellRadius; dy++)
                {
                    long key = MakeKey(cx + dx, cy + dy);
                    if (!_cells.TryGetValue(key, out var list))
                        continue;

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (count >= maxResults)
                        {
#if UNITY_EDITOR
                            WarnTruncation(maxResults);
#endif
                            return count;
                        }
                        results[count++] = list[i];
                    }
                }
            }

            return count;
        }

#if UNITY_EDITOR
        // 节流警告：避免每帧触发導致 Console 刷屏
        private double _lastWarnTime = -999;
        private void WarnTruncation(int bufferSize)
        {
            double now = Time.realtimeSinceStartupAsDouble;
            if (now - _lastWarnTime < 1.0) return;
            _lastWarnTime = now;
            Debug.LogWarning(
                $"[SpatialHashGrid] QueryNeighbors 缓冲区已满（{bufferSize}），" +
                "查询范围内仍有实体未被写入，存在静默漏检。" +
                "请增大调用方的 _neighborBuffer 大小。");
        }
#endif


        // ------------------------------------------------
        // 内部辅助
        // ------------------------------------------------
        private int CellX(float x) => (int)Math.Floor(x * _invCellSize);
        private int CellY(float y) => (int)Math.Floor(y * _invCellSize);

        private static long MakeKey(int cx, int cy)
            => ((long)cx << 32) | (uint)cy;
    }
}
