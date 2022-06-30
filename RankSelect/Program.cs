// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace RankSelect
{
    /**
     * This is a horrible implementation of a bit vector for convenience only.
     *
     * The point is to experiment with the RankSelect algorithm.
     *
     * Real implementation should actually use bits, and do not construct the chunk objects, ...
     *
     */
    public sealed class BitVector
    {
        private bool[] bits;
        private Chunk[]? chunks;

        private int n;
        private int chunkSize;
        private int subChunkSize;

        public BitVector(int length)
        {
            this.bits = new bool[length];

            n = this.bits.Length;
            int log = (int)Math.Log2(n);
            chunkSize = log * log;
            subChunkSize = log / 2;

            bool debug = false;

            /*
             * For debugging only, feel free to change the chunkSize and subChunkSize without "logical" consequence
             * Of course, it will have a size consequence
             */
             if (debug)
             {
                
                chunkSize = 4;
                subChunkSize = 2;
             }
        }

        public void Set(int index, bool value)
        {
            this.bits[index] = value;
        }

        public void BuildRankSupport()
        {
            int numberOfChunks = n / chunkSize;
            if (n % chunkSize != 0)
            {
                numberOfChunks++;
            }


            int numberOfSubChunks = chunkSize / subChunkSize;
            if (chunkSize % subChunkSize != 0)
            {
                numberOfSubChunks++;
            }


            chunks = new Chunk[numberOfChunks];

            int scanningIndex = 0;
            int rankSoFar = 0;
            for (int chunkIndex = 0; chunkIndex < numberOfChunks; chunkIndex++)
            {
                int chunkFrom = chunkIndex * chunkSize;
                int chunkTo = chunkFrom + chunkSize;
                chunks[chunkIndex] = new Chunk(chunkFrom, chunkTo);
                chunks[chunkIndex].rank = rankSoFar;
                chunks[chunkIndex].subChunks = new SubChunk[numberOfSubChunks];
                int subRankSoFar = 0;
                for (int subChunksIndex = 0; subChunksIndex < numberOfSubChunks; subChunksIndex++)
                {
                    int subChunkFrom = chunkFrom + subChunksIndex * subChunkSize;
                    int subChunkTo = subChunkFrom + subChunkSize;
                    chunks![chunkIndex]!.subChunks![subChunksIndex] = new SubChunk(subChunkFrom, subChunkTo);
                    chunks![chunkIndex]!.subChunks![subChunksIndex]!.subRank = subRankSoFar;
                    for (int i = 0; i < subChunkSize; i++)
                    {
                        if (scanningIndex == n) {
                            break;
                        }
                        int bit = (this.bits[scanningIndex] ? 1 : 0);
                        rankSoFar += bit;
                        subRankSoFar += bit;
                        scanningIndex++;
                    }
                }
            }
        }

        private sealed class Chunk
        {
            public int from;
            public int to;
            public int rank;

            public Chunk(int from, int to)
            {
                this.from = from;
                this.to = to;
            }

            public SubChunk[]? subChunks;
        }

        private sealed class SubChunk
        {
            public int from;
            public int to;
            public int subRank;

            public SubChunk(int from, int to)
            {
                this.from = from;
                this.to = to;
            }
        }

        public void ShowRankSupport()
        {
            for (int chunkIndex = 0; chunkIndex < this.chunks!.Length; chunkIndex++)
            {
                Chunk chunk = chunks![chunkIndex]!;
                SubChunk[] subChunks = chunk.subChunks!;
                Console.WriteLine($"Chunk #{chunkIndex}: [{chunk.from}, {chunk.to}) has rank {chunk.rank}");
                for (int subChunksIndex = 0; subChunksIndex < subChunks.Length; subChunksIndex++)
                {
                    SubChunk subChunk = subChunks[subChunksIndex]!;
                    Console.WriteLine($"  SubChunk #{subChunksIndex}: [{subChunk.from}, {subChunk.to}) has subRank {subChunk.subRank}");
                }
            }
        }

        public void Rank(int position)
        {
            if (0 > position || position >= n)
            {
                throw new ArgumentException($"{nameof(position)} out of bound");
            }

            Console.WriteLine();
            Console.WriteLine($"Performing a rank operation on {position}");
            Console.WriteLine();

            int whichChunk = position / chunkSize;
            Console.WriteLine($"We are in chunk #{whichChunk}");

            int withinChunk = position % chunkSize;
            int whichSubChunk = withinChunk / subChunkSize;
            Console.WriteLine($"We are in subchunk #{whichSubChunk}");

            Chunk chunk = this.chunks![whichChunk]!;
            int chunkRank = chunk.rank;
            Console.WriteLine($"The chunk rank is {chunkRank}");

            SubChunk subChunk = chunk.subChunks![whichSubChunk]!;
            int subChunkSubRank = subChunk.subRank;
            Console.WriteLine($"The subchunk subrank is {subChunkSubRank}");

            int subChunkFrom = subChunk.from;
            Console.WriteLine($"The subChunk starts at position {subChunkFrom}");

            // Technically, the paper said we should build a lookup table to ensure O(1) query.
            // But I find it really silly do to so, since subchunk usually have tiny size
            // Even for 4G bits, log (2^32) / 2 = 16, and scanning 16 bits takes next to no time,
            // It really doesn't make sense to create that lookup table at all.

            int rank = chunkRank + subChunkSubRank;
            Console.WriteLine($"We knew the subChunk start at rank={rank}");
            for (int i = subChunkFrom; i <= position; i++)
            {
                rank += (this.bits[i] ? 1 : 0);
            }
            Console.WriteLine($"After counting within the subChunk, we know the answer is {rank}");
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            BitVector bits = new BitVector(16);
            bits.Set(5, true);
            bits.BuildRankSupport();
            bits.ShowRankSupport();
            bits.Rank(7);
        }
    }
}
