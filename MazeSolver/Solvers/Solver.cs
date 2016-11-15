using Microsoft.Xna.Framework;
using PepesComing.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PepesComing {
    public abstract class Solver {
        protected Thread solveThread;
        protected World world;

        private Stopwatch timer;
        protected SolverMouse mouse;
        public SolverMouse Mouse {
            get {
                return mouse;
            }
        }

        protected bool[,] _solution;
        public bool[,] Solution {
            get {
                return _solution;
            }
        }

        // Gets the time elapsed since the solver started
        public long Elapsed {
            get {
                return timer.ElapsedMilliseconds;
            }
        }

        public Solver(ref World world) {
            _solution = new bool[World.width, World.height];


            timer = Stopwatch.StartNew();

            // Start solving maze
            solveThread = new Thread(Solve);
            solveThread.IsBackground = true;
            solveThread.Start();
            this.world = world;
        }

        private void Solve() {
            while (!Done()) {
                Step();
            }
            timer.Stop();
        }

        public abstract void Step();
        public abstract bool Done();
    }
}
