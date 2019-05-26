using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GraphX;

namespace CSG
{
    public class Clipper : MarshalByRefObject
    {
        /// <summary>
        /// Metoda znajdowania przecięcia odcinków s1s2 i c1c2. Użyj jej w etapie 1.
        /// Metoda wypełnia pole Distance w zwracanych wierzchołkach, czyli względną pozycję wierzchołka na odcinku.
        /// </summary>
        /// <param name="s1">Początek pierwszego odcinka</param>
        /// <param name="s2">Koniec pierwszego odcinka</param>
        /// <param name="c1">Początek drugiego odcinka</param>
        /// <param name="c2">Koniec drugiego odcinka</param>
        /// <returns>Zwraca dwa wierzchołki, które mają te same współrzędne, 
        /// ale różnią się względną pozycją na swoim odcinku (pierwszy zwracany wierzchołek leży na s1s2,
        /// a drugi na c1c2). Gdy odcinki się nie przecinają, zwracany jest null.</returns>
        public (Vertex, Vertex)? GetIntersectionPoints(Vertex s1, Vertex s2, Vertex c1, Vertex c2)
        {
            double d = (c2.Y - c1.Y) * (s2.X - s1.X) - (c2.X - c1.X) * (s2.Y - s1.Y);
            if (d == 0.0)
                return null;
            double toSource = ((c2.X - c1.X) * (s1.Y - c1.Y) - (c2.Y - c1.Y) * (s1.X - c1.X)) / d;
            double toClip = ((s2.X - s1.X) * (s1.Y - c1.Y) - (s2.Y - s1.Y) * (s1.X - c1.X)) / d;

            Vertex s = new Vertex(s1.X + toSource * (s2.X - s1.X), s1.Y + toSource * (s2.Y - s1.Y))
            {
                Distance = toSource,
                IsIntersection = true
            };
            Vertex c = new Vertex(s1.X + toSource * (s2.X - s1.X), s1.Y + toSource * (s2.Y - s1.Y))
            {
                Distance = toClip,
                IsIntersection = true
            };

            if ((0 < toSource && toSource < 1) && (0 < toClip && toClip < 1))
            {
                return (s, c);
            }
            else
                return null;
        }

        /// <summary>
        /// Metoda sprawdzająca, czy dany wierzchołek v znajduje się w danym wielokącie p. Użyj w etapach 2.
        /// </summary>
        /// <param name="v">Wierzchołek</param>
        /// <param name="p">Wielokąt</param>
        /// <returns>Prawda, jeśli wierzchołek znajduje się wewnątrz wielokąta, fałsz w przeciwnym wypadku</returns>
        public bool IsInside(LinkedListNode<Vertex> v, Polygon p)
        {
            // funkcja strzela nieskończoną prostą w lewo (ujemne X-y) od punktu v
            // jeśli na swojej drodze napotka nieparzystą liczbę boków, to znaczy, że jest w środku wielokąta p
            // w praktyce odbywa się to tak, że sprawdzamy, czy dany bok przecina Y-ową współrzędną punktu,
            // jednocześnie (używając interpolacji liniowej) czy to miejsce w odcinku, na który pada rzut punktu v na oś OY
            // jest po lewej (po ludzku: czy odcinek jest na lewo od punktu). 
            // Jeśli tak, to jest to bok przecinający się z nieskończoną prostą idącą w lewo. Inne boki nas nie interesują

            bool oddNodes = false;
            double x = v.Value.X;
            double y = v.Value.Y;

            // znów nie da się sprytnie foreachem, bo trzeba mieć dostęp także do następnego
            for (LinkedListNode<Vertex> LLvertex = p.Vertices.First; LLvertex != null; LLvertex = LLvertex.Next)
            {
                Vertex vertex;
                Vertex next;

                //if ma zapewnić, że sprawdzimy także bok łączący ostatni wierzchołek z pierwszym 
                if (LLvertex.Next == null)
                {
                    vertex = LLvertex.Value;
                    next = p.Vertices.First.Value;
                }
                else
                {
                    vertex = LLvertex.Value;
                    next = LLvertex.Next.Value;
                }

                // czy odcinek przecina Y-ową współrzędną punktu?
                // czy choć jedna współrzędna X-owa odcinka jest po lewej?
                if ((vertex.Y < y && next.Y >= y ||
                       next.Y < y && vertex.Y >= y) &&
                    (vertex.X <= x || next.X <= x))
                {
                    // jeśli tak, to czy odcinek jest na lewo od punktu?
                    oddNodes ^= vertex.X + (y - vertex.Y) / (next.Y - vertex.Y) * (next.X - vertex.X) < x;
                }
            }

            return oddNodes;
        }

        /// <summary>
        /// Metoda znajdowania punktów, gdzie przecinają się dwa wielokąty ze sobą. 
        /// Argumenty nie są modyfikowane, zmodyfikowane wersje są zwracane jako wynik.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Zwraca zmodyfikowane kopie wielokątów wejściowych</returns>
        public (Polygon, Polygon) MakeIntersectionPoints(Polygon source, Polygon clip)
        {
            Polygon sourceCopy = new Polygon(source);
            Polygon clipCopy = new Polygon(clip);

            var v1 = sourceCopy.Vertices.First;

            while (v1 != null)
            {
                var v1_next = v1.Next ?? v1.List.First;
                var v2 = clipCopy.Vertices.First;
                while (v2 != null)
                {
                    var v2_next = v2.Next ?? v2.List.First;
                    //przecinanie
                    var result = GetIntersectionPoints(v1.Value, v1_next.Value, v2.Value, v2_next.Value);
                    if (result != null)
                    {
                        var result1 = result.Value.Item1;
                        var result2 = result.Value.Item2;
                        sourceCopy.Vertices.AddAfter(v1, result1);
                        clipCopy.Vertices.AddAfter(v2, result2);
                        LinkedListNode<Vertex> linkedv1 = v1.Next;
                        LinkedListNode<Vertex> linkedv2 = v2.Next;
                        v1_next = linkedv1;
                        v2_next = linkedv2;
                        linkedv1.Value.CorrespondingVertex = linkedv2;
                        linkedv2.Value.CorrespondingVertex = linkedv1;

                    }
                    //
                    v2 = v2.Next;
                }

                v1 = v1.Next;
            }

            return (sourceCopy, clipCopy);
        }

        public (Polygon, Polygon) MakeIntersectionPoints2(Polygon source, Polygon clip)
        {
            Polygon sourceCopy = new Polygon(source);
            Polygon clipCopy = new Polygon(clip);

            var v1 = sourceCopy.Vertices.First;
            

            while(v1.Next!= null)
            {
                var v1_next = v1.Next ?? v1.List.First;
                var v2 = clipCopy.Vertices.First;
                while (v2 != null)
                {
                    var v2_next = v2.Next ?? v2.List.First;
                    //przecinanie
                    var result = GetIntersectionPoints(v1.Value, v1_next.Value, v2.Value, v2_next.Value);
                    if(result != null)
                    {
                        var result1 = result.Value.Item1;
                        var result2 = result.Value.Item2;
                        //sourceCopy.Vertices.AddAfter(v1, result1);
                        //clipCopy.Vertices.AddAfter(v2, result2);
                        LinkedListNode<Vertex> linkedv1 = null;
                        LinkedListNode<Vertex> linkedv2 = null;
                        bool added1 = false, added2 = false;
                        var v1temp = v1.Next ?? v1.List.First;
                        while (v1temp != v1_next)
                        {
                            if(result1.Distance < v1temp.Value.Distance)
                            {
                                sourceCopy.Vertices.AddBefore(v1temp, result1);
                                linkedv1 = v1temp.Previous;
                                added1 = true;
                                break;
                            }
                            v1temp = v1temp.Next;
                        }

                        var v2temp = v2.Next ?? v2.List.First;
                        while (v2temp != v2_next)
                        {
                            if (result2.Distance < v2temp.Value.Distance)
                            {
                                sourceCopy.Vertices.AddBefore(v2temp, result2);
                                linkedv2 = v2temp.Previous;
                                added2 = true;
                                break;
                            }
                            v2temp = v2temp.Next;
                        }
                        if (!added1)
                        {
                            sourceCopy.Vertices.AddBefore(v1_next, result1);
                            linkedv1 = v1_next.Previous;
                        }
                        if (!added2)
                        {
                            clipCopy.Vertices.AddBefore(v2_next, result2);
                            linkedv2 = v2_next.Previous;
                        }
                        linkedv1.Value.CorrespondingVertex = linkedv2;
                        linkedv2.Value.CorrespondingVertex = linkedv1;

                    }
                    //
                    v2 = v2.Next;
                }

                v1 = v1.Next;
            }

            return (sourceCopy, clipCopy);
        }

        /// <summary>
        /// Metoda oznaczająca wierzchołki jako wejściowe lub wyjściowe.
        /// Argumenty nie są modyfikowane, zmodyfikowane wersje są zwracane jako wynik.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Zwraca zmodyfikowane kopie wielokątów wejściowych</returns>
        public (Polygon, Polygon) MarkEntryExitPoints(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = MakeIntersectionPoints(source, clip);

            var v = sourceCopy.Vertices.First;
            var v1 = v;
            while (v != null)
            {
               if  (!(IsInside(v, clipCopy)) && !clipCopy.Vertices.Any(x => x == v.Value))
                {
                    v1 = v;
                    break;
                }
                v = v.Next ;
            }

            bool inside = false;
            for(int i =0;i<sourceCopy.Vertices.Count;i++)
            {
                if (v1.Value.IsIntersection)
                {
                    v1.Value.IsEntry = !inside ;
                    inside = !inside;
                }
                v1 = v1.Next ?? sourceCopy.Vertices.First;
            }


            v = clipCopy.Vertices.First;
            var v2 = v;
            while (v != null)
            {
                if (!(IsInside(v, sourceCopy)) && ! sourceCopy.Vertices.Any(x => x==v.Value) )
                {
                    v2 = v;
                    break;
                }
                v = v.Next;
            }

            inside = false;
            for (int i = 0; i < clipCopy.Vertices.Count; i++)
            {
                if (v2.Value.IsIntersection)
                {
                    v2.Value.IsEntry = !inside;
                    inside = !inside;
                }
                v2 = v2.Next ?? clipCopy.Vertices.First;
            }
            
            return (sourceCopy,clipCopy);
        }

        /// <summary>
        /// Metoda zwracająca wynik operacji logicznej na dwóch wielokątach.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Lista wynikowych wielokątów</returns>
        public List<Polygon> ReturnClippedPolygons(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = MarkEntryExitPoints(source, clip);
            List<LinkedListNode<Vertex>> IntersectionPoints = new List<LinkedListNode<Vertex>>();
            for(var iter = sourceCopy.Vertices.First; iter != null; iter = iter.Next)
            {
                if(iter.Value.IsIntersection)
                    IntersectionPoints.Add(iter);
            }
            List<Polygon> result = new List<Polygon>();
            if (IntersectionPoints.Count == 0)
            {
                if (IsInside(sourceCopy.Vertices.First, clip)) { result.Add(sourceCopy);  return result; }
                else if (IsInside(clipCopy.Vertices.First, source)) { result.Add(clipCopy);  return result; }
                return result;
            }

            while(IntersectionPoints.Count != 0)
            {
                var polygon = new Polygon();
                var entry = IntersectionPoints.First(/*x => x.Value.IsEntry*/);
                var first = entry;
            et:
                IntersectionPoints.Remove(entry);
                polygon.Vertices.AddLast(new Vertex(entry.Value.X,entry.Value.Y ));
                bool go_ahead = entry.Value.IsEntry;
                do
                {
                    entry = go_ahead ? (entry.Next ?? entry.List.First ): (entry.Previous ?? entry.List.Last);
                    polygon.Vertices.AddLast(new Vertex(entry.Value.X, entry.Value.Y));
                }
                while (!entry.Value.IsIntersection);
                IntersectionPoints.Remove(entry);
                entry = entry.Value.CorrespondingVertex;
                go_ahead = entry.Value.IsEntry;
                do
                {
                    entry = go_ahead ?( entry.Next ?? entry.List.First) : (entry.Previous ?? entry.List.Last);
                    polygon.Vertices.AddLast(new Vertex(entry.Value.X, entry.Value.Y));
                }
                while (!entry.Value.IsIntersection);
                polygon.Vertices.RemoveLast();
                if (entry.Value != first.Value) {
                    entry = entry.Value.CorrespondingVertex;
                    goto et; }
                result.Add(polygon);
                
            }

            return result;
        }
    }
}
