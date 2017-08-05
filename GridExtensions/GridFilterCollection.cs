namespace GridExtensions
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    /// <summary>
    ///     Typesafe collection of <see cref="IGridFilter" />s.
    /// </summary>
    public class GridFilterCollection : ReadOnlyCollectionBase
    {
        private readonly Hashtable columnStylesToGridFiltersHash;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="columnStyles">
        ///     List of <see cref="DataGridColumnStyle" /> which are associated with
        ///     <see cref="IGridFilter" />s.
        /// </param>
        /// <param name="columnStylesToGridFiltersHash">
        ///     Mapping between <see cref="DataGridColumnStyle" /> and
        ///     <see cref="IGridFilter" />s.
        /// </param>
        internal GridFilterCollection(IList columnStyles, Hashtable columnStylesToGridFiltersHash)
        {
            this.columnStylesToGridFiltersHash = (Hashtable)columnStylesToGridFiltersHash.Clone();

            foreach (DataGridColumnStyle columnStyle in columnStyles)
            {
                var gridFilter = (IGridFilter)this.columnStylesToGridFiltersHash[columnStyle];
                if (gridFilter != null) this.InnerList.Add(gridFilter);
            }
        }

        /// <summary>
        ///     Gets the element at the specified index.
        /// </summary>
        public IGridFilter this[int index] => (IGridFilter)this.InnerList[index];

        /// <summary>
        ///     Gets the <see cref="IGridFilter" /> which is associated with the given <see cref="DataGridColumnStyle" />.
        /// </summary>
        public IGridFilter this[DataGridColumnStyle columnStyle]
        {
            get
            {
                if (this.InnerList.Contains(this.columnStylesToGridFiltersHash[columnStyle]))
                    return (IGridFilter)this.columnStylesToGridFiltersHash[columnStyle];
                return null;
            }
        }

        /// <summary>
        ///     Determines whether a given <see cref="IGridFilter" /> is contained in this instance.
        /// </summary>
        /// <param name="gridFilter"><see cref="IGridFilter" /> to be checked.</param>
        /// <returns>True if <see cref="IGridFilter" /> is contained in this instance otherwise False.</returns>
        public bool Contains(IGridFilter gridFilter)
        {
            return this.InnerList.Contains(gridFilter);
        }

        /// <summary>
        ///     Creates a filtered list which only contains <see cref="IGridFilter" />s of the specified type.
        /// </summary>
        /// <param name="dataType">Type by which should be filtered.</param>
        /// <param name="exactMatch">
        ///     Defines whether the types must match exactly
        ///     (otherwise inheriting types will also be returned).
        /// </param>
        /// <returns>Collection of matching <see cref="IGridFilter" />s.</returns>
        public GridFilterCollection FilterByGridFilterType(Type dataType, bool exactMatch)
        {
            if (!typeof(IGridFilter).IsAssignableFrom(dataType))
                throw new ArgumentException("Given type must implement IGridFilter.", nameof(dataType));
            var filtered = new ArrayList();
            foreach (DataGridColumnStyle columnStyle in this.columnStylesToGridFiltersHash.Keys)
                if (this[columnStyle] != null && (this[columnStyle].GetType() == dataType
                                                  || !exactMatch && dataType.IsInstanceOfType(this[columnStyle])))
                    filtered.Add(columnStyle);

            return new GridFilterCollection(filtered, this.columnStylesToGridFiltersHash);
        }

        /// <summary>
        ///     Gets a <see cref="IGridFilter" /> which is associated with a <see cref="DataGridColumnStyle" />
        ///     with the specified <see cref="DataGridColumnStyle.HeaderText" />.
        /// </summary>
        /// <param name="headerText">
        ///     <see cref="DataGridColumnStyle.HeaderText" />
        /// </param>
        /// <returns>An <see cref="IGridFilter" /> or null if no appropriate was found.</returns>
        public IGridFilter GetByHeaderText(string headerText)
        {
            foreach (DataGridColumnStyle columnStyle in this.columnStylesToGridFiltersHash.Keys)
                if (columnStyle.HeaderText == headerText) return this[columnStyle];
            return null;
        }

        /// <summary>
        ///     Gets a <see cref="IGridFilter" /> which is associated with a <see cref="DataGridColumnStyle" />
        ///     with the specified <see cref="DataGridColumnStyle.MappingName" />.
        /// </summary>
        /// <param name="mappingName">
        ///     <see cref="DataGridColumnStyle.MappingName" />
        /// </param>
        /// <returns>An <see cref="IGridFilter" /> or null if no appropriate was found.</returns>
        public IGridFilter GetByMappingName(string mappingName)
        {
            foreach (DataGridColumnStyle columnStyle in this.columnStylesToGridFiltersHash.Keys)
                if (columnStyle.MappingName == mappingName) return this[columnStyle];
            return null;
        }
    }
}