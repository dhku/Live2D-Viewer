using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class UI
    {
        protected GameObject canvas;
        protected bool isVisible = false;
        protected VtuberCore manager = null;

        public UI(GameObject canvas)
        {
            this.manager = VtuberCore.instance;
            this.canvas = canvas;
            this.canvas.SetActive(this.isVisible);
            this.load();
        }

        public abstract void load();

        public void SetVisible(bool isVisible)
        {
            this.canvas.SetActive(isVisible);
            this.isVisible = isVisible;
        }

        public bool IsVisible()
        {
            return this.isVisible;
        }

    }
}
