using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using System;

namespace Hierarchy
{
    [Serializable]
    public class BSP
    {
        public BSPNode m_Root;

        public BSPNode Root => m_Root;

        public BSP(Model model)
        {
            BuildTree(model);
        }

        public BSP(BSPNode node)
        {
            m_Root = node;
        }

        public void BuildTree(Model model)
        {
            m_Root = new BSPNode(model.ToPolygons());
        }

        public BSP Clone()
        {
            BSP bsp = new BSP(m_Root.Clone());
            return bsp;
        }
    }
}
