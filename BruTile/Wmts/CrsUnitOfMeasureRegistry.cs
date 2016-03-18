// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;

namespace BruTile.Wmts
{
    /// <summary>
    /// A unit of measure
    /// </summary>
    public struct UnitOfMeasure : IEquatable<UnitOfMeasure>
    {
        private readonly double _toMeter;

        /// <summary>
        /// Initializes this unit of measue with a <paramref name="name"/> and <paramref name="toMeter"/> value.
        /// </summary>
        /// <param name="name">A value indicating the name of the unit of measure</param>
        /// <param name="toMeter">A scale value to transform this Unit of measure</param>
        internal UnitOfMeasure(string name, double toMeter)
        {
            Name = name;
            _toMeter = toMeter;
        }

        /// <summary>
        /// Gets a value indicating the name of the unit of measure
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a scale value to transform this Unit of measure to <see cref="Meter"/>
        /// </summary>
        public double ToMeter => _toMeter;

        /// <summary>
        /// Function to check for equality 
        /// </summary>
        /// <param name="other">Another unit of measure</param>
        /// <returns><c>true</c> if <see cref="ToMeter"/>s are equal</returns>
        public bool Equals(UnitOfMeasure other)
        {
            return _toMeter == other.ToMeter;
        }

    }

    public class CrsUnitOfMeasureRegistry
    {
        private static readonly Dictionary<int, UnitOfMeasure> Registry = new Dictionary<int, UnitOfMeasure>();

        private const double EarthRadius = 6378137;
        private const double EarthCircumference = 2*EarthRadius*Math.PI;
        private const double EarthArc = EarthCircumference/(2*Math.PI);
        private static readonly byte[] EpsgToUomData = 
        {
18, 8, 79, 35, 88, 8, 134, 35, 107, 8, 43, 35, 111, 8, 134, 35, 
112, 8, 134, 35, 146, 8, 43, 35, 156, 8, 43, 35, 174, 8, 42, 35, 
175, 8, 42, 35, 176, 8, 42, 35, 177, 8, 43, 35, 178, 8, 43, 35, 
179, 8, 43, 35, 180, 8, 43, 35, 181, 8, 43, 35, 182, 8, 43, 35, 
183, 8, 43, 35, 184, 8, 43, 35, 185, 8, 43, 35, 186, 8, 43, 35, 
187, 8, 43, 35, 188, 8, 43, 35, 189, 8, 43, 35, 190, 8, 43, 35, 
191, 8, 43, 35, 192, 8, 43, 35, 193, 8, 43, 35, 194, 8, 43, 35, 
195, 8, 43, 35, 196, 8, 43, 35, 197, 8, 43, 35, 198, 8, 43, 35, 
199, 8, 43, 35, 200, 8, 43, 35, 201, 8, 43, 35, 202, 8, 43, 35, 
203, 8, 42, 35, 204, 8, 42, 35, 205, 8, 42, 35, 206, 8, 43, 35, 
207, 8, 43, 35, 208, 8, 42, 35, 209, 8, 43, 35, 210, 8, 43, 35, 
211, 8, 43, 35, 212, 8, 43, 35, 213, 8, 43, 35, 214, 8, 43, 35, 
215, 8, 43, 35, 216, 8, 43, 35, 217, 8, 42, 35, 218, 8, 42, 35, 
219, 8, 43, 35, 220, 8, 43, 35, 221, 8, 42, 35, 222, 8, 42, 35, 
223, 8, 43, 35, 224, 8, 43, 35, 225, 8, 42, 35, 226, 8, 43, 35, 
227, 8, 43, 35, 228, 8, 43, 35, 229, 8, 43, 35, 230, 8, 43, 35, 
231, 8, 43, 35, 232, 8, 42, 35, 233, 8, 42, 35, 234, 8, 42, 35, 
235, 8, 43, 35, 236, 8, 43, 35, 237, 8, 43, 35, 238, 8, 43, 35, 
239, 8, 43, 35, 240, 8, 43, 35, 241, 8, 43, 35, 10, 9, 45, 35, 
51, 11, 42, 35, 52, 11, 42, 35, 53, 11, 42, 35, 54, 11, 43, 35, 
55, 11, 43, 35, 56, 11, 43, 35, 57, 11, 43, 35, 58, 11, 43, 35, 
59, 11, 43, 35, 60, 11, 43, 35, 61, 11, 43, 35, 62, 11, 43, 35, 
63, 11, 43, 35, 64, 11, 43, 35, 65, 11, 43, 35, 66, 11, 43, 35, 
67, 11, 43, 35, 68, 11, 43, 35, 69, 11, 43, 35, 70, 11, 43, 35, 
71, 11, 43, 35, 72, 11, 43, 35, 73, 11, 43, 35, 74, 11, 43, 35, 
75, 11, 43, 35, 76, 11, 43, 35, 77, 11, 43, 35, 78, 11, 43, 35, 
79, 11, 43, 35, 80, 11, 42, 35, 81, 11, 42, 35, 82, 11, 42, 35, 
83, 11, 43, 35, 84, 11, 43, 35, 85, 11, 42, 35, 86, 11, 43, 35, 
87, 11, 43, 35, 88, 11, 43, 35, 89, 11, 43, 35, 90, 11, 43, 35, 
91, 11, 43, 35, 92, 11, 43, 35, 93, 11, 42, 35, 94, 11, 42, 35, 
95, 11, 43, 35, 96, 11, 43, 35, 97, 11, 42, 35, 98, 11, 42, 35, 
99, 11, 43, 35, 100, 11, 43, 35, 101, 11, 43, 35, 102, 11, 43, 35, 
103, 11, 43, 35, 104, 11, 43, 35, 105, 11, 42, 35, 106, 11, 42, 35, 
107, 11, 42, 35, 108, 11, 43, 35, 109, 11, 43, 35, 110, 11, 43, 35, 
111, 11, 43, 35, 112, 11, 43, 35, 113, 11, 43, 35, 114, 11, 43, 35, 
148, 11, 43, 35, 149, 11, 43, 35, 150, 11, 43, 35, 151, 11, 43, 35, 
152, 11, 43, 35, 176, 11, 42, 35, 178, 11, 42, 35, 8, 12, 42, 35, 
17, 12, 43, 35, 19, 12, 43, 35, 30, 12, 43, 35, 67, 12, 138, 35, 
68, 12, 138, 35, 95, 12, 85, 36, 31, 13, 42, 35, 33, 13, 42, 35, 
35, 13, 43, 35, 37, 13, 43, 35, 76, 13, 43, 35, 79, 13, 45, 35, 
89, 13, 43, 35, 90, 13, 43, 35, 91, 13, 43, 35, 92, 13, 43, 35, 
93, 13, 43, 35, 94, 13, 43, 35, 95, 13, 43, 35, 96, 13, 43, 35, 
97, 13, 43, 35, 98, 13, 43, 35, 99, 13, 43, 35, 100, 13, 43, 35, 
101, 13, 43, 35, 102, 13, 43, 35, 103, 13, 43, 35, 104, 13, 43, 35, 
105, 13, 43, 35, 106, 13, 43, 35, 107, 13, 43, 35, 108, 13, 43, 35, 
109, 13, 43, 35, 110, 13, 43, 35, 113, 13, 43, 35, 114, 13, 43, 35, 
115, 13, 43, 35, 116, 13, 43, 35, 117, 13, 43, 35, 118, 13, 43, 35, 
123, 13, 43, 35, 124, 13, 43, 35, 125, 13, 43, 35, 126, 13, 43, 35, 
127, 13, 43, 35, 128, 13, 43, 35, 129, 13, 43, 35, 130, 13, 43, 35, 
131, 13, 43, 35, 151, 13, 42, 35, 153, 13, 42, 35, 155, 13, 42, 35, 
157, 13, 43, 35, 159, 13, 43, 35, 162, 13, 43, 35, 164, 13, 43, 35, 
166, 13, 43, 35, 168, 13, 43, 35, 170, 13, 43, 35, 172, 13, 43, 35, 
174, 13, 43, 35, 176, 13, 43, 35, 178, 13, 43, 35, 180, 13, 43, 35, 
182, 13, 43, 35, 184, 13, 43, 35, 187, 13, 43, 35, 189, 13, 43, 35, 
191, 13, 43, 35, 193, 13, 43, 35, 195, 13, 43, 35, 197, 13, 43, 35, 
199, 13, 43, 35, 201, 13, 43, 35, 203, 13, 43, 35, 205, 13, 43, 35, 
207, 13, 43, 35, 209, 13, 43, 35, 211, 13, 43, 35, 213, 13, 43, 35, 
215, 13, 43, 35, 217, 13, 43, 35, 219, 13, 43, 35, 221, 13, 43, 35, 
223, 13, 43, 35, 225, 13, 43, 35, 232, 13, 43, 35, 233, 13, 43, 35, 
234, 13, 43, 35, 235, 13, 43, 35, 236, 13, 43, 35, 237, 13, 43, 35, 
238, 13, 43, 35, 239, 13, 43, 35, 240, 13, 43, 35, 241, 13, 43, 35, 
242, 13, 43, 35, 254, 13, 43, 35, 0, 14, 43, 35, 2, 14, 43, 35, 
4, 14, 42, 35, 6, 14, 42, 35, 9, 14, 42, 35, 14, 14, 43, 35, 
16, 14, 43, 35, 21, 14, 42, 35, 24, 14, 43, 35, 26, 14, 43, 35, 
28, 14, 43, 35, 30, 14, 43, 35, 32, 14, 43, 35, 34, 14, 43, 35, 
36, 14, 43, 35, 38, 14, 43, 35, 40, 14, 43, 35, 42, 14, 43, 35, 
44, 14, 43, 35, 46, 14, 43, 35, 48, 14, 43, 35, 50, 14, 42, 35, 
52, 14, 42, 35, 56, 14, 43, 35, 58, 14, 43, 35, 60, 14, 42, 35, 
62, 14, 42, 35, 64, 14, 42, 35, 66, 14, 43, 35, 68, 14, 43, 35, 
70, 14, 43, 35, 72, 14, 42, 35, 74, 14, 43, 35, 76, 14, 43, 35, 
78, 14, 43, 35, 80, 14, 43, 35, 84, 14, 43, 35, 86, 14, 43, 35, 
88, 14, 43, 35, 90, 14, 43, 35, 92, 14, 42, 35, 93, 14, 43, 35, 
95, 14, 42, 35, 96, 14, 43, 35, 98, 14, 42, 35, 99, 14, 43, 35, 
102, 14, 43, 35, 104, 14, 43, 35, 106, 14, 43, 35, 108, 14, 43, 35, 
112, 14, 43, 35, 114, 14, 43, 35, 116, 14, 43, 35, 144, 14, 43, 35, 
145, 14, 43, 35, 146, 14, 43, 35, 147, 14, 43, 35, 148, 14, 43, 35, 
149, 14, 43, 35, 150, 14, 43, 35, 151, 14, 43, 35, 152, 14, 43, 35, 
153, 14, 43, 35, 154, 14, 43, 35, 155, 14, 43, 35, 169, 14, 43, 35, 
170, 14, 43, 35, 171, 14, 43, 35, 172, 14, 43, 35, 173, 14, 43, 35, 
174, 14, 43, 35, 175, 14, 43, 35, 176, 14, 43, 35, 235, 14, 162, 35, 
237, 14, 162, 35, 239, 14, 162, 35, 240, 14, 162, 35, 48, 15, 162, 35, 
49, 15, 162, 35, 66, 15, 162, 35, 151, 15, 43, 35, 152, 15, 43, 35, 
161, 15, 162, 35, 162, 15, 162, 35, 163, 15, 162, 35, 164, 15, 162, 35, 
165, 15, 162, 35, 166, 15, 162, 35, 167, 15, 162, 35, 168, 15, 162, 35, 
169, 15, 162, 35, 170, 15, 162, 35, 171, 15, 162, 35, 172, 15, 162, 35, 
173, 15, 162, 35, 174, 15, 162, 35, 175, 15, 162, 35, 176, 15, 162, 35, 
177, 15, 162, 35, 178, 15, 162, 35, 179, 15, 162, 35, 180, 15, 162, 35, 
181, 15, 162, 35, 182, 15, 162, 35, 183, 15, 162, 35, 184, 15, 162, 35, 
185, 15, 162, 35, 187, 15, 162, 35, 188, 15, 162, 35, 189, 15, 162, 35, 
190, 15, 162, 35, 191, 15, 162, 35, 192, 15, 162, 35, 193, 15, 162, 35, 
194, 15, 162, 35, 195, 15, 142, 35, 196, 15, 162, 35, 200, 15, 162, 35, 
201, 15, 162, 35, 202, 15, 162, 35, 203, 15, 162, 35, 204, 15, 162, 35, 
205, 15, 162, 35, 206, 15, 162, 35, 207, 15, 162, 35, 212, 15, 162, 35, 
213, 15, 162, 35, 214, 15, 162, 35, 215, 15, 162, 35, 234, 15, 162, 35, 
235, 15, 162, 35, 240, 15, 162, 35, 241, 15, 162, 35, 24, 16, 162, 35, 
25, 16, 162, 35, 26, 16, 162, 35, 27, 16, 162, 35, 28, 16, 162, 35, 
29, 16, 142, 35, 30, 16, 142, 35, 31, 16, 162, 35, 32, 16, 162, 35, 
33, 16, 162, 35, 34, 16, 162, 35, 35, 16, 162, 35, 36, 16, 162, 35, 
37, 16, 162, 35, 38, 16, 162, 35, 39, 16, 162, 35, 40, 16, 162, 35, 
41, 16, 162, 35, 42, 16, 162, 35, 43, 16, 162, 35, 44, 16, 142, 35, 
45, 16, 162, 35, 46, 16, 162, 35, 47, 16, 162, 35, 48, 16, 162, 35, 
49, 16, 162, 35, 50, 16, 162, 35, 51, 16, 162, 35, 52, 16, 162, 35, 
53, 16, 162, 35, 54, 16, 162, 35, 55, 16, 162, 35, 56, 16, 162, 35, 
57, 16, 162, 35, 58, 16, 162, 35, 59, 16, 162, 35, 60, 16, 162, 35, 
61, 16, 162, 35, 62, 16, 162, 35, 63, 16, 162, 35, 64, 16, 162, 35, 
65, 16, 162, 35, 66, 16, 162, 35, 67, 16, 162, 35, 68, 16, 162, 35, 
69, 16, 162, 35, 70, 16, 162, 35, 71, 16, 162, 35, 72, 16, 162, 35, 
73, 16, 162, 35, 74, 16, 162, 35, 75, 16, 162, 35, 76, 16, 142, 35, 
77, 16, 162, 35, 78, 16, 162, 35, 79, 16, 162, 35, 80, 16, 162, 35, 
82, 16, 162, 35, 83, 16, 162, 35, 84, 16, 162, 35, 85, 16, 162, 35, 
86, 16, 162, 35, 87, 16, 162, 35, 88, 16, 162, 35, 89, 16, 142, 35, 
92, 16, 162, 35, 93, 16, 162, 35, 94, 16, 162, 35, 95, 16, 162, 35, 
96, 16, 162, 35, 97, 16, 162, 35, 98, 16, 162, 35, 99, 16, 162, 35, 
100, 16, 162, 35, 101, 16, 162, 35, 102, 16, 162, 35, 103, 16, 162, 35, 
104, 16, 162, 35, 105, 16, 162, 35, 106, 16, 162, 35, 107, 16, 162, 35, 
108, 16, 162, 35, 109, 16, 162, 35, 110, 16, 162, 35, 111, 16, 162, 35, 
112, 16, 162, 35, 113, 16, 162, 35, 114, 16, 162, 35, 115, 16, 162, 35, 
116, 16, 162, 35, 117, 16, 162, 35, 118, 16, 162, 35, 119, 16, 162, 35, 
120, 16, 162, 35, 121, 16, 43, 35, 122, 16, 162, 35, 123, 16, 162, 35, 
124, 16, 162, 35, 125, 16, 162, 35, 126, 16, 162, 35, 127, 16, 162, 35, 
128, 16, 162, 35, 129, 16, 162, 35, 130, 16, 142, 35, 131, 16, 162, 35, 
132, 16, 142, 35, 133, 16, 162, 35, 134, 16, 162, 35, 135, 16, 162, 35, 
136, 16, 162, 35, 137, 16, 162, 35, 138, 16, 142, 35, 139, 16, 142, 35, 
140, 16, 162, 35, 141, 16, 162, 35, 142, 16, 162, 35, 143, 16, 162, 35, 
144, 16, 162, 35, 145, 16, 162, 35, 146, 16, 162, 35, 147, 16, 162, 35, 
148, 16, 162, 35, 149, 16, 162, 35, 150, 16, 162, 35, 151, 16, 162, 35, 
152, 16, 162, 35, 153, 16, 162, 35, 154, 16, 162, 35, 155, 16, 162, 35, 
156, 16, 162, 35, 157, 16, 162, 35, 158, 16, 162, 35, 159, 16, 162, 35, 
160, 16, 162, 35, 161, 16, 162, 35, 162, 16, 162, 35, 163, 16, 162, 35, 
164, 16, 142, 35, 165, 16, 162, 35, 166, 16, 162, 35, 167, 16, 162, 35, 
168, 16, 162, 35, 169, 16, 162, 35, 170, 16, 162, 35, 171, 16, 162, 35, 
172, 16, 162, 35, 173, 16, 162, 35, 174, 16, 162, 35, 175, 16, 162, 35, 
176, 16, 162, 35, 177, 16, 162, 35, 178, 16, 162, 35, 179, 16, 162, 35, 
180, 16, 162, 35, 181, 16, 162, 35, 182, 16, 162, 35, 183, 16, 162, 35, 
184, 16, 162, 35, 185, 16, 162, 35, 186, 16, 162, 35, 187, 16, 162, 35, 
188, 16, 162, 35, 189, 16, 162, 35, 190, 16, 162, 35, 191, 16, 142, 35, 
192, 16, 162, 35, 193, 16, 162, 35, 195, 16, 142, 35, 196, 16, 162, 35, 
197, 16, 162, 35, 198, 16, 142, 35, 199, 16, 162, 35, 200, 16, 142, 35, 
201, 16, 162, 35, 202, 16, 162, 35, 203, 16, 162, 35, 204, 16, 162, 35, 
205, 16, 162, 35, 206, 16, 162, 35, 207, 16, 162, 35, 208, 16, 162, 35, 
210, 16, 162, 35, 211, 16, 162, 35, 212, 16, 162, 35, 213, 16, 162, 35, 
214, 16, 162, 35, 215, 16, 162, 35, 216, 16, 162, 35, 217, 16, 162, 35, 
218, 16, 162, 35, 219, 16, 162, 35, 220, 16, 162, 35, 221, 16, 162, 35, 
222, 16, 162, 35, 223, 16, 162, 35, 226, 16, 162, 35, 228, 16, 162, 35, 
230, 16, 162, 35, 231, 16, 142, 35, 233, 16, 142, 35, 243, 16, 142, 35, 
245, 16, 142, 35, 247, 16, 142, 35, 249, 16, 142, 35, 251, 16, 162, 35, 
253, 16, 142, 35, 255, 16, 142, 35, 1, 17, 142, 35, 3, 17, 142, 35, 
5, 17, 142, 35, 7, 17, 142, 35, 9, 17, 142, 35, 11, 17, 142, 35, 
13, 17, 142, 35, 15, 17, 142, 35, 17, 17, 142, 35, 19, 17, 142, 35, 
21, 17, 142, 35, 23, 17, 142, 35, 25, 17, 142, 35, 27, 17, 142, 35, 
29, 17, 142, 35, 31, 17, 142, 35, 34, 17, 142, 35, 36, 17, 142, 35, 
47, 17, 43, 35, 48, 17, 43, 35, 49, 17, 43, 35, 50, 17, 43, 35, 
51, 17, 43, 35, 52, 17, 43, 35, 53, 17, 43, 35, 54, 17, 43, 35, 
55, 17, 43, 35, 56, 17, 43, 35, 57, 17, 43, 35, 58, 17, 43, 35, 
59, 17, 43, 35, 60, 17, 43, 35, 61, 17, 43, 35, 66, 17, 43, 35, 
67, 17, 43, 35, 68, 17, 43, 35, 69, 17, 43, 35, 70, 17, 43, 35, 
71, 17, 43, 35, 72, 17, 43, 35, 73, 17, 43, 35, 74, 17, 43, 35, 
75, 17, 43, 35, 76, 17, 43, 35, 77, 17, 43, 35, 78, 17, 43, 35, 
79, 17, 43, 35, 80, 17, 43, 35, 81, 17, 43, 35, 86, 17, 43, 35, 
87, 17, 43, 35, 103, 17, 43, 35, 104, 17, 43, 35, 105, 17, 43, 35, 
111, 17, 162, 35, 114, 17, 162, 35, 117, 17, 162, 35, 118, 17, 162, 35, 
120, 17, 162, 35, 123, 17, 162, 35, 128, 17, 162, 35, 130, 17, 162, 35, 
131, 17, 162, 35, 138, 17, 162, 35, 203, 17, 162, 35, 205, 17, 162, 35, 
206, 17, 162, 35, 248, 17, 162, 35, 249, 17, 162, 35, 250, 17, 162, 35, 
251, 17, 162, 35, 252, 17, 162, 35, 253, 17, 162, 35, 254, 17, 162, 35, 
255, 17, 162, 35, 0, 18, 162, 35, 1, 18, 162, 35, 2, 18, 162, 35, 
3, 18, 162, 35, 4, 18, 162, 35, 5, 18, 162, 35, 6, 18, 162, 35, 
7, 18, 162, 35, 8, 18, 162, 35, 9, 18, 162, 35, 10, 18, 162, 35, 
11, 18, 162, 35, 12, 18, 162, 35, 13, 18, 162, 35, 14, 18, 162, 35, 
15, 18, 162, 35, 16, 18, 162, 35, 17, 18, 162, 35, 18, 18, 162, 35, 
19, 18, 162, 35, 20, 18, 162, 35, 21, 18, 162, 35, 22, 18, 162, 35, 
23, 18, 162, 35, 24, 18, 162, 35, 25, 18, 162, 35, 26, 18, 142, 35, 
27, 18, 162, 35, 28, 18, 162, 35, 29, 18, 162, 35, 30, 18, 162, 35, 
31, 18, 162, 35, 32, 18, 162, 35, 33, 18, 162, 35, 34, 18, 162, 35, 
35, 18, 162, 35, 36, 18, 162, 35, 37, 18, 162, 35, 38, 18, 162, 35, 
49, 18, 162, 35, 50, 18, 162, 35, 51, 18, 162, 35, 52, 18, 162, 35, 
53, 18, 162, 35, 54, 18, 162, 35, 55, 18, 162, 35, 56, 18, 162, 35, 
57, 18, 162, 35, 58, 18, 162, 35, 59, 18, 162, 35, 60, 18, 162, 35, 
61, 18, 162, 35, 62, 18, 162, 35, 63, 18, 162, 35, 64, 18, 162, 35, 
65, 18, 162, 35, 66, 18, 162, 35, 67, 18, 162, 35, 68, 18, 162, 35, 
69, 18, 162, 35, 70, 18, 162, 35, 71, 18, 162, 35, 72, 18, 162, 35, 
73, 18, 162, 35, 74, 18, 162, 35, 75, 18, 162, 35, 76, 18, 162, 35, 
77, 18, 162, 35, 78, 18, 162, 35, 79, 18, 162, 35, 80, 18, 162, 35, 
81, 18, 162, 35, 82, 18, 162, 35, 83, 18, 162, 35, 84, 18, 162, 35, 
85, 18, 162, 35, 86, 18, 162, 35, 87, 18, 162, 35, 88, 18, 162, 35, 
89, 18, 162, 35, 90, 18, 162, 35, 91, 18, 162, 35, 92, 18, 162, 35, 
93, 18, 162, 35, 94, 18, 162, 35, 95, 18, 162, 35, 96, 18, 162, 35, 
97, 18, 162, 35, 98, 18, 162, 35, 99, 18, 162, 35, 100, 18, 162, 35, 
101, 18, 162, 35, 102, 18, 162, 35, 103, 18, 162, 35, 104, 18, 162, 35, 
105, 18, 162, 35, 106, 18, 162, 35, 107, 18, 162, 35, 108, 18, 162, 35, 
109, 18, 162, 35, 110, 18, 162, 35, 111, 18, 162, 35, 112, 18, 162, 35, 
113, 18, 162, 35, 114, 18, 162, 35, 115, 18, 162, 35, 116, 18, 162, 35, 
117, 18, 162, 35, 118, 18, 162, 35, 119, 18, 162, 35, 120, 18, 162, 35, 
121, 18, 162, 35, 122, 18, 162, 35, 123, 18, 162, 35, 124, 18, 162, 35, 
125, 18, 162, 35, 126, 18, 162, 35, 127, 18, 162, 35, 128, 18, 162, 35, 
129, 18, 162, 35, 130, 18, 162, 35, 131, 18, 162, 35, 132, 18, 162, 35, 
133, 18, 162, 35, 134, 18, 162, 35, 135, 18, 162, 35, 136, 18, 162, 35, 
137, 18, 162, 35, 138, 18, 162, 35, 139, 18, 162, 35, 140, 18, 162, 35, 
141, 18, 162, 35, 142, 18, 162, 35, 143, 18, 162, 35, 144, 18, 162, 35, 
145, 18, 162, 35, 146, 18, 162, 35, 147, 18, 162, 35, 148, 18, 162, 35, 
149, 18, 162, 35, 150, 18, 162, 35, 151, 18, 162, 35, 152, 18, 162, 35, 
153, 18, 162, 35, 154, 18, 162, 35, 155, 18, 162, 35, 156, 18, 162, 35, 
157, 18, 162, 35, 193, 18, 162, 35, 194, 18, 162, 35, 195, 18, 162, 35, 
196, 18, 162, 35, 197, 18, 162, 35, 198, 18, 162, 35, 199, 18, 145, 35, 
200, 18, 162, 35, 201, 18, 162, 35, 202, 18, 145, 35, 203, 18, 145, 35, 
205, 18, 162, 35, 206, 18, 162, 35, 207, 18, 162, 35, 208, 18, 145, 35, 
209, 18, 162, 35, 210, 18, 162, 35, 211, 18, 145, 35, 212, 18, 162, 35, 
213, 18, 145, 35, 215, 18, 162, 35, 216, 18, 162, 35, 19, 19, 162, 35, 
21, 19, 162, 35, 23, 19, 162, 35, 25, 19, 162, 35, 27, 19, 162, 35, 
29, 19, 162, 35, 31, 19, 162, 35, 34, 19, 162, 35, 36, 19, 162, 35, 
37, 19, 145, 35, 38, 19, 145, 35, 39, 19, 162, 35, 40, 19, 162, 35, 
43, 19, 162, 35, 45, 19, 162, 35, 57, 19, 162, 35, 59, 19, 162, 35, 
61, 19, 162, 35, 63, 19, 162, 35, 65, 19, 162, 35, 67, 19, 162, 35, 
69, 19, 162, 35, 71, 19, 162, 35, 73, 19, 162, 35, 75, 19, 162, 35, 
77, 19, 162, 35, 79, 19, 162, 35, 81, 19, 162, 35, 83, 19, 162, 35, 
85, 19, 162, 35, 87, 19, 162, 35, 89, 19, 162, 35, 91, 19, 162, 35, 
93, 19, 162, 35, 95, 19, 162, 35, 97, 19, 162, 35, 99, 19, 162, 35, 
101, 19, 162, 35, 103, 19, 162, 35, 105, 19, 162, 35, 107, 19, 162, 35, 
109, 19, 162, 35, 111, 19, 162, 35, 113, 19, 162, 35, 115, 19, 162, 35, 
117, 19, 162, 35, 119, 19, 162, 35, 121, 19, 162, 35, 123, 19, 162, 35, 
125, 19, 162, 35, 127, 19, 162, 35, 129, 19, 162, 35, 131, 19, 162, 35, 
133, 19, 162, 35, 135, 19, 162, 35, 148, 19, 162, 35, 149, 19, 162, 35, 
12, 20, 162, 35, 108, 20, 162, 35, 109, 20, 162, 35, 113, 20, 162, 35, 
125, 20, 162, 35, 126, 20, 162, 35, 131, 20, 162, 35, 132, 20, 162, 35, 
143, 20, 162, 35, 144, 20, 162, 35, 203, 20, 162, 35, 204, 20, 162, 35, 
220, 20, 162, 35, 222, 20, 162, 35, 233, 20, 162, 35, 234, 20, 162, 35, 
239, 20, 162, 35, 240, 20, 162, 35, 244, 20, 162, 35, 245, 20, 162, 35, 
250, 20, 162, 35, 251, 20, 162, 35, 252, 20, 162, 35, 253, 20, 162, 35, 
4, 21, 162, 35, 5, 21, 162, 35, 16, 21, 162, 35, 17, 21, 162, 35, 
75, 21, 162, 35, 88, 21, 162, 35, 91, 21, 162, 35, 96, 21, 77, 35, 
112, 21, 162, 35, 113, 21, 162, 35, 148, 21, 162, 35, 151, 21, 162, 35, 
169, 21, 162, 35, 170, 21, 162, 35, 184, 21, 162, 35, 185, 21, 162, 35, 
212, 21, 42, 35, 213, 21, 45, 35, 216, 21, 162, 35, 217, 21, 162, 35, 
238, 21, 42, 35, 247, 21, 43, 35, 248, 21, 43, 35, 249, 21, 43, 35, 
14, 22, 43, 35, 22, 22, 43, 35, 23, 22, 43, 35, 49, 22, 162, 35, 
70, 22, 43, 35, 122, 22, 135, 35, 172, 22, 249, 35, 173, 22, 245, 35, 
186, 22, 0, 4, 198, 22, 162, 35, 227, 22, 0, 4, 253, 22, 162, 35, 
254, 22, 162, 35, 240, 23, 42, 35, 241, 23, 42, 35, 242, 23, 42, 35, 
243, 23, 42, 35, 244, 23, 42, 35, 246, 23, 162, 35, 247, 23, 162, 35, 
253, 23, 42, 35, 56, 24, 43, 35, 57, 24, 43, 35, 58, 24, 43, 35, 
63, 24, 162, 35, 174, 24, 162, 35, 175, 24, 162, 35, 177, 24, 162, 35, 
178, 24, 162, 35, 180, 24, 162, 35, 181, 24, 162, 35, 214, 24, 43, 35, 
215, 24, 43, 35, 216, 24, 43, 35, 220, 24, 162, 35, 221, 24, 162, 35, 
247, 24, 42, 35, 5, 25, 42, 35, 7, 25, 42, 35, 9, 25, 42, 35, 
11, 25, 43, 35, 13, 25, 43, 35, 16, 25, 43, 35, 18, 25, 43, 35, 
20, 25, 43, 35, 22, 25, 43, 35, 24, 25, 43, 35, 26, 25, 43, 35, 
28, 25, 43, 35, 30, 25, 43, 35, 32, 25, 43, 35, 34, 25, 43, 35, 
36, 25, 43, 35, 38, 25, 43, 35, 41, 25, 43, 35, 43, 25, 43, 35, 
45, 25, 43, 35, 47, 25, 43, 35, 49, 25, 43, 35, 51, 25, 43, 35, 
53, 25, 43, 35, 55, 25, 43, 35, 57, 25, 43, 35, 59, 25, 43, 35, 
61, 25, 43, 35, 63, 25, 43, 35, 65, 25, 43, 35, 67, 25, 43, 35, 
69, 25, 43, 35, 71, 25, 43, 35, 73, 25, 43, 35, 75, 25, 43, 35, 
77, 25, 43, 35, 79, 25, 43, 35, 84, 25, 43, 35, 86, 25, 43, 35, 
88, 25, 43, 35, 90, 25, 43, 35, 92, 25, 43, 35, 94, 25, 42, 35, 
96, 25, 42, 35, 99, 25, 42, 35, 101, 25, 43, 35, 103, 25, 43, 35, 
105, 25, 43, 35, 107, 25, 43, 35, 110, 25, 43, 35, 115, 25, 42, 35, 
117, 25, 43, 35, 119, 25, 43, 35, 121, 25, 43, 35, 123, 25, 43, 35, 
125, 25, 43, 35, 127, 25, 43, 35, 129, 25, 43, 35, 131, 25, 43, 35, 
133, 25, 43, 35, 135, 25, 43, 35, 137, 25, 43, 35, 139, 25, 43, 35, 
141, 25, 43, 35, 143, 25, 43, 35, 145, 25, 42, 35, 147, 25, 42, 35, 
149, 25, 43, 35, 151, 25, 43, 35, 153, 25, 43, 35, 155, 25, 43, 35, 
157, 25, 42, 35, 159, 25, 42, 35, 161, 25, 42, 35, 163, 25, 43, 35, 
165, 25, 43, 35, 168, 25, 43, 35, 170, 25, 42, 35, 172, 25, 43, 35, 
174, 25, 43, 35, 176, 25, 43, 35, 178, 25, 43, 35, 182, 25, 43, 35, 
184, 25, 43, 35, 186, 25, 43, 35, 188, 25, 43, 35, 190, 25, 43, 35, 
193, 25, 43, 35, 195, 25, 43, 35, 197, 25, 43, 35, 199, 25, 43, 35, 
201, 25, 43, 35, 203, 25, 43, 35, 205, 25, 43, 35, 207, 25, 43, 35, 
209, 25, 43, 35, 212, 25, 43, 35, 214, 25, 43, 35, 216, 25, 43, 35, 
218, 25, 43, 35, 225, 25, 43, 35, 226, 25, 43, 35, 227, 25, 43, 35, 
233, 25, 43, 35, 28, 87, 76, 35, 36, 94, 45, 35, 50, 95, 124, 35, 
51, 95, 124, 35, 52, 95, 124, 35, 53, 95, 124, 35, 54, 95, 124, 35, 
62, 95, 124, 35, 251, 95, 102, 35, 105, 104, 43, 35, 106, 104, 43, 35, 
107, 104, 43, 35, 108, 104, 43, 35, 109, 104, 43, 35, 110, 104, 43, 35, 
111, 104, 43, 35, 112, 104, 43, 35, 113, 104, 43, 35, 114, 104, 43, 35, 
115, 104, 43, 35, 116, 104, 43, 35, 117, 104, 43, 35, 118, 104, 43, 35, 
119, 104, 43, 35, 120, 104, 43, 35, 121, 104, 43, 35, 122, 104, 43, 35, 
123, 104, 43, 35, 124, 104, 43, 35, 125, 104, 43, 35, 126, 104, 43, 35, 
127, 104, 43, 35, 128, 104, 43, 35, 129, 104, 43, 35, 130, 104, 43, 35, 
131, 104, 43, 35, 132, 104, 43, 35, 133, 104, 43, 35, 134, 104, 43, 35, 
135, 104, 43, 35, 136, 104, 43, 35, 142, 104, 43, 35, 143, 104, 43, 35, 
144, 104, 43, 35, 145, 104, 43, 35, 146, 104, 43, 35, 147, 104, 43, 35, 
148, 104, 43, 35, 149, 104, 43, 35, 150, 104, 43, 35, 151, 104, 43, 35, 
152, 104, 43, 35, 153, 104, 43, 35, 154, 104, 43, 35, 155, 104, 43, 35, 
156, 104, 43, 35, 157, 104, 43, 35, 158, 104, 43, 35, 159, 104, 43, 35, 
160, 104, 43, 35, 161, 104, 43, 35, 162, 104, 43, 35, 163, 104, 43, 35, 
167, 104, 43, 35, 168, 104, 43, 35, 169, 104, 43, 35, 170, 104, 43, 35, 
171, 104, 43, 35, 172, 104, 43, 35, 173, 104, 43, 35, 174, 104, 43, 35, 
175, 104, 43, 35, 177, 104, 43, 35, 178, 104, 43, 35, 179, 104, 43, 35, 
187, 104, 43, 35, 188, 104, 43, 35, 189, 104, 43, 35, 223, 104, 43, 35, 
224, 104, 43, 35, 225, 104, 43, 35, 226, 104, 43, 35, 227, 104, 43, 35, 
228, 104, 43, 35, 229, 104, 43, 35, 230, 104, 43, 35, 231, 104, 43, 35, 
232, 104, 43, 35, 233, 104, 43, 35, 234, 104, 43, 35, 235, 104, 43, 35, 
236, 104, 43, 35, 237, 104, 43, 35, 238, 104, 43, 35, 239, 104, 43, 35, 
240, 104, 43, 35, 241, 104, 43, 35, 242, 104, 43, 35, 243, 104, 43, 35, 
244, 104, 43, 35, 245, 104, 43, 35, 246, 104, 43, 35, 155, 106, 80, 35, 
156, 106, 80, 35, 187, 114, 71, 35, 189, 114, 71, 35, 191, 114, 71, 35, 
193, 114, 71, 35, 195, 114, 71, 35, 197, 114, 71, 35, 199, 114, 71, 35, 
201, 114, 71, 35, 175, 116, 82, 35, 176, 116, 81, 35, 248, 117, 79, 35, 
1, 125, 43, 35, 2, 125, 43, 35, 3, 125, 43, 35, 5, 125, 43, 35, 
6, 125, 43, 35, 7, 125, 43, 35, 8, 125, 43, 35, 9, 125, 43, 35, 
10, 125, 43, 35, 11, 125, 43, 35, 12, 125, 43, 35, 13, 125, 43, 35, 
14, 125, 43, 35, 15, 125, 43, 35, 16, 125, 43, 35, 17, 125, 43, 35, 
18, 125, 43, 35, 19, 125, 43, 35, 20, 125, 43, 35, 21, 125, 43, 35, 
22, 125, 43, 35, 23, 125, 43, 35, 24, 125, 43, 35, 25, 125, 43, 35, 
26, 125, 43, 35, 27, 125, 43, 35, 28, 125, 43, 35, 29, 125, 43, 35, 
30, 125, 43, 35, 31, 125, 43, 35, 33, 125, 43, 35, 34, 125, 43, 35, 
35, 125, 43, 35, 36, 125, 43, 35, 37, 125, 43, 35, 38, 125, 43, 35, 
39, 125, 43, 35, 40, 125, 43, 35, 41, 125, 43, 35, 42, 125, 43, 35, 
43, 125, 43, 35, 44, 125, 43, 35, 45, 125, 43, 35, 46, 125, 43, 35, 
47, 125, 43, 35, 48, 125, 43, 35, 49, 125, 43, 35, 50, 125, 43, 35, 
51, 125, 43, 35, 52, 125, 43, 35, 53, 125, 43, 35, 54, 125, 43, 35, 
55, 125, 43, 35, 56, 125, 43, 35, 57, 125, 43, 35, 58, 125, 43, 35, 
64, 125, 43, 35, 65, 125, 43, 35, 66, 125, 43, 35, 67, 125, 43, 35, 
74, 125, 43, 35, 75, 125, 43, 35, 76, 125, 43, 35, 77, 125, 43, 35, 
99, 125, 43, 35, 164, 125, 43, 35, 165, 125, 43, 35, 166, 125, 43, 35, 
167, 125, 43, 35, 152, 127, 43, 35, 153, 127, 43, 35, 154, 127, 43, 35, 
155, 127, 43, 35
        };
        
        static CrsUnitOfMeasureRegistry()
        {
            Registry = new Dictionary<int, UnitOfMeasure>();
            Registry.Add(1024, new UnitOfMeasure("bin", 1d));
            Registry.Add(9201, new UnitOfMeasure("unity", 1d));
            Registry.Add(1025, new UnitOfMeasure("millimetre", 1d/1000d));
            Registry.Add(1033, new UnitOfMeasure("centimetre", 1d/100d));
            Registry.Add(9001, new UnitOfMeasure("metre", 1d/1d));
            Registry.Add(9002, new UnitOfMeasure("foot", 0.3048d/1d));
            Registry.Add(9003, new UnitOfMeasure("US survey foot", 12d/39.37d));
            Registry.Add(9005, new UnitOfMeasure("Clarke's foot", 0.3047972654d/1d));
            Registry.Add(9014, new UnitOfMeasure("fathom", 1.8288d/1d));
            Registry.Add(9030, new UnitOfMeasure("nautical mile", 1852d/1d));
            Registry.Add(9031, new UnitOfMeasure("German legal metre", 1.0000135965d/1d));
            Registry.Add(9033, new UnitOfMeasure("US survey chain", 792d/39.37d));
            Registry.Add(9034, new UnitOfMeasure("US survey link", 7.92d/39.37d));
            Registry.Add(9035, new UnitOfMeasure("US survey mile", 63360d/39.37d));
            Registry.Add(9036, new UnitOfMeasure("kilometre", 1000d/1d));
            Registry.Add(9037, new UnitOfMeasure("Clarke's yard", 0.9143917962d/1d));
            Registry.Add(9038, new UnitOfMeasure("Clarke's chain", 20.1166195164d/1d));
            Registry.Add(9039, new UnitOfMeasure("Clarke's link", 0.201166195164d/1d));
            Registry.Add(9040, new UnitOfMeasure("British yard (Sears 1922)", 36d/39.370147d));
            Registry.Add(9041, new UnitOfMeasure("British foot (Sears 1922)", 12d/39.370147d));
            Registry.Add(9042, new UnitOfMeasure("British chain (Sears 1922)", 792d/39.370147d));
            Registry.Add(9043, new UnitOfMeasure("British link (Sears 1922)", 7.92d/39.370147d));
            Registry.Add(9050, new UnitOfMeasure("British yard (Benoit 1895 A)", 0.9143992d/1d));
            Registry.Add(9051, new UnitOfMeasure("British foot (Benoit 1895 A)", 0.9143992d/3d));
            Registry.Add(9052, new UnitOfMeasure("British chain (Benoit 1895 A)", 20.1167824d/1d));
            Registry.Add(9053, new UnitOfMeasure("British link (Benoit 1895 A)", 0.201167824d/1d));
            Registry.Add(9060, new UnitOfMeasure("British yard (Benoit 1895 B)", 36d/39.370113d));
            Registry.Add(9061, new UnitOfMeasure("British foot (Benoit 1895 B)", 12d/39.370113d));
            Registry.Add(9062, new UnitOfMeasure("British chain (Benoit 1895 B)", 792d/39.370113d));
            Registry.Add(9063, new UnitOfMeasure("British link (Benoit 1895 B)", 7.92d/39.370113d));
            Registry.Add(9070, new UnitOfMeasure("British foot (1865)", 0.9144025d/3d));
            Registry.Add(9080, new UnitOfMeasure("Indian foot", 12d/39.370142d));
            Registry.Add(9081, new UnitOfMeasure("Indian foot (1937)", 0.30479841d/1d));
            Registry.Add(9082, new UnitOfMeasure("Indian foot (1962)", 0.3047996d/1d));
            Registry.Add(9083, new UnitOfMeasure("Indian foot (1975)", 0.3047995d/1d));
            Registry.Add(9084, new UnitOfMeasure("Indian yard", 36d/39.370142d));
            Registry.Add(9085, new UnitOfMeasure("Indian yard (1937)", 0.91439523d/1d));
            Registry.Add(9086, new UnitOfMeasure("Indian yard (1962)", 0.9143988d/1d));
            Registry.Add(9087, new UnitOfMeasure("Indian yard (1975)", 0.9143985d/1d));
            Registry.Add(9093, new UnitOfMeasure("Statute mile", 1609.344d/1d));
            Registry.Add(9094, new UnitOfMeasure("Gold Coast foot", 6378300d/20926201d));
            Registry.Add(9095, new UnitOfMeasure("British foot (1936)", 0.3048007491d/1d));
            Registry.Add(9096, new UnitOfMeasure("yard", 0.9144d/1d));
            Registry.Add(9097, new UnitOfMeasure("chain", 20.1168d/1d));
            Registry.Add(9098, new UnitOfMeasure("link", 20.1168d/100d));
            Registry.Add(9099, new UnitOfMeasure("British yard (Sears 1922 truncated)", 0.914398d/1d));
            Registry.Add(9204, new UnitOfMeasure("Bin width 330 US survey feet", 3960d/39.37d));
            Registry.Add(9205, new UnitOfMeasure("Bin width 165 US survey feet", 1980d/39.37d));
            Registry.Add(9206, new UnitOfMeasure("Bin width 82.5 US survey feet", 990d/39.37d));
            Registry.Add(9207, new UnitOfMeasure("Bin width 37.5 metres", 37.5d/1d));
            Registry.Add(9208, new UnitOfMeasure("Bin width 25 metres", 25d/1d));
            Registry.Add(9209, new UnitOfMeasure("Bin width 12.5 metres", 12.5d/1d));
            Registry.Add(9210, new UnitOfMeasure("Bin width 6.25 metres", 6.25d/1d));
            Registry.Add(9211, new UnitOfMeasure("Bin width 3.125 metres", 3.125d/1d));
            Registry.Add(9300, new UnitOfMeasure("British foot (Sears 1922 truncated)", 0.914398d/3d));
            Registry.Add(9301, new UnitOfMeasure("British chain (Sears 1922 truncated)", 20.116756d/1d));
            Registry.Add(9302, new UnitOfMeasure("British link (Sears 1922 truncated)", 20.116756d/100d));
            Registry.Add(1031, new UnitOfMeasure("milliarc-second", EarthArc*3.14159265358979d/648000000d));
            Registry.Add(9101, new UnitOfMeasure("radian", EarthArc*1d/1d));
            Registry.Add(9102, new UnitOfMeasure("degree", EarthArc*3.14159265358979d/180d));
            Registry.Add(9103, new UnitOfMeasure("arc-minute", EarthArc*3.14159265358979d/10800d));
            Registry.Add(9104, new UnitOfMeasure("arc-second", EarthArc*3.14159265358979d/648000d));
            Registry.Add(9105, new UnitOfMeasure("grad", EarthArc*3.14159265358979d/200d));
            Registry.Add(9106, new UnitOfMeasure("gon", EarthArc*3.14159265358979d/200d));
            Registry.Add(9109, new UnitOfMeasure("microradian", EarthArc*1d/1000000d));
            Registry.Add(9112, new UnitOfMeasure("centesimal minute", EarthArc*3.14159265358979d/20000d));
            Registry.Add(9113, new UnitOfMeasure("centesimal second", EarthArc*3.14159265358979d/2000000d));
            Registry.Add(9114, new UnitOfMeasure("mil_6400", EarthArc*3.14159265358979d/3200d));
            Registry.Add(9122, new UnitOfMeasure("degree (supplier to define representation)", EarthArc*3.14159265358979d/180d));
        }

        public UnitOfMeasure this[int index] => Registry[index];

        public UnitOfMeasure this[CrsIdentifier identifier]
        {
            get
            {
                switch (identifier.Authority.ToUpper())
                {
                    case "OGC":
                        if (identifier.Equals(WellKnownScaleSets.CRS84))
                            return this[9102];
                        return this[9001];
                    case "EPSG":
                        return this[SeekUom(int.Parse(identifier.Identifier))];

                }
                throw new ArgumentException("identifier");
            }
        }

        private static readonly Dictionary<int, int> EpsgToUom = new Dictionary<int, int>();
        private static int SeekUom(int epsgCode)
        {
            lock (EpsgToUom)
            {
                int resUom;
                if (EpsgToUom.TryGetValue(epsgCode, out resUom))
                    return resUom;

                resUom = 9001;
   
                using (var br = new BinaryReader(new MemoryStream(EpsgToUomData)))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        var srid = br.ReadUInt16();
                        if (srid > epsgCode) break;
                        var uom = br.ReadInt16();
                        if (srid != epsgCode)
                            continue;
                        
                        resUom = uom;
                        break;
                    }
                }

                EpsgToUom.Add(epsgCode, resUom);
                return resUom;
            }
        }
    }
}
