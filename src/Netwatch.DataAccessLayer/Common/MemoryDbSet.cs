#region Copyright (C) 2014 Netwatch

// Copyright (C) 2014 Netwatch
// https://github.com/flumbee/netwatch

// This file is part of Netwatch

// Applified.NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Netwatch.DataAccessLayer.Common
{
    public class MemoryDbSet<T> : IDbSet<T> where T : class
    {
        private readonly HashSet<T> _data;
        private readonly IQueryable _query;
        private int _identity = 1;
        private List<PropertyInfo> _keyProperties;

        public MemoryDbSet()
        {
            IEnumerable<T> startData = null;
            GetKeyProperties();
            _data = (startData != null ? new HashSet<T>(startData) : new HashSet<T>());
            _query = _data.AsQueryable();
        }

        public virtual T Find(params object[] keyValues)
        {
            if (keyValues.Length != _keyProperties.Count)
                throw new ArgumentException("Incorrect number of keys passed to find method");

            var keyQuery = this.AsQueryable();
            for (var i = 0; i < keyValues.Length; i++)
            {
                var x = i; // nested linq
                keyQuery = keyQuery.Where(entity => _keyProperties[x].GetValue(entity, null).Equals(keyValues[x]));
            }

            return keyQuery.SingleOrDefault();
        }

        public T Add(T item)
        {
            GenerateId(item);
            _data.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public ObservableCollection<T> Local
        {
            get { return new ObservableCollection<T>(_data); }
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        private void GetKeyProperties()
        {
            _keyProperties = new List<PropertyInfo>();
            var properties = typeof (T).GetProperties();
            foreach (var property in properties)
            {
                foreach (Attribute attribute in property.GetCustomAttributes(true))
                {
                    if (attribute is KeyAttribute)
                    {
                        _keyProperties.Add(property);
                    }
                }
            }
        }

        private void GenerateId(T entity)
        {
            // If non-composite integer key
            if (_keyProperties.Count == 1 && _keyProperties[0].PropertyType == typeof (Int32))
                _keyProperties[0].SetValue(entity, _identity++, null);
        }

        public void Detach(T item)
        {
            _data.Remove(item);
        }
    }
}