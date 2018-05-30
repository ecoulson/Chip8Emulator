using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Chip8.Desktop.Emulator
{
    public class Chip8Keyboard
    {
        private static Dictionary<Keys, int> keys = new Dictionary<Keys, int>() {
            { Keys.D1, 1  },
            { Keys.D2, 2  },
            { Keys.D3, 3  },
            { Keys.D4, 12 },
            { Keys.Q, 4   },
            { Keys.W, 5   },
            { Keys.E, 6   },
            { Keys.R, 13  },
            { Keys.A, 7   },
            { Keys.S, 8   },
            { Keys.D, 9   },
            { Keys.F, 14  },
            { Keys.Z, 10  },
            { Keys.X, 0   },
            { Keys.C, 11  },
            { Keys.V, 15  }
        };

        public bool[] keyState;

        public Chip8Keyboard() {
            this.keyState = new bool[keys.Count];
        }

        public void setState(KeyboardState state) {
            foreach (KeyValuePair<Keys, int> pair in keys) {
                if (state.IsKeyDown(pair.Key)) {
                    keyState[pair.Value] = true;
                } else {
                    keyState[pair.Value] = false;
                }
            }
        }

    }
}
