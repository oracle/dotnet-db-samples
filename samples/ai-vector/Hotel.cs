using Microsoft.Extensions.VectorData;

namespace OracleAIVectorData
{
    public class Hotel
    {
        [VectorStoreKey]
        public int HotelId { get; set; }

        [VectorStoreData]
        public string HotelName { get; set; }

        [VectorStoreData]
        public float Rating { get; set; }

        [VectorStoreData]
        public bool HasParking { get; set; }

        [VectorStoreData]
        public string Description { get; set; }

        //Oracle has numerous vector distance functions to identify the most relevant results.
        //Let's use cosine similarity for the hotel name vectors.
        [VectorStoreVector(Dimensions: 384, DistanceFunction = DistanceFunction.CosineDistance)]
        public float[] NameEmbedding { get; set; }

        //Let's use Euclidean distance for the hotel description vectors.
        [VectorStoreVector(Dimensions: 384, DistanceFunction = DistanceFunction.EuclideanDistance)]
        public float[] DescriptionEmbedding { get; set; }
    }
}
/* Copyright (c) 2025 Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/