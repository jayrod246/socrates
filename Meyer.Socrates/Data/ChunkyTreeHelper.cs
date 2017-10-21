namespace Meyer.Socrates.Data
{
    using Meyer.Socrates.Data.Sections;
    using Meyer.Socrates.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ChunkyTreeHelper
    {
        private const string NOSINGLEPARENT = "source has no single parent.";
        private const string NOSINGLEPARENT_WITHCONTSTRAINTS = "source has no single parent that matches the constraints given.";
        private const string NOSINGLECHILD = "source has no single child.";
        private const string NOSINGLECHILD_WITHCONTSTRAINTS = "source has no single child that matches the constraints given.";

        /// <summary>
        /// Returns a single parent chunk. Throws if more than one parent exists, or if no parent was found.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Chunk GetParent(this Chunk source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var parent = ParentsOfChunk(source).SingleOrDefault();
            if (parent == null) throw new ArgumentException(NOSINGLEPARENT, "source");
            return parent;
        }

        /// <summary>
        /// Returns a single parent chunk with Quad = <paramref name="quad"/>. Throws if more than one parent matches the type constraint, or if no parent was found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="quad"></param>
        /// <returns></returns>
        public static Chunk GetParent(this Chunk source, string quad)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var parent = ParentsOfChunk(source).Where(c => c.Quad == quad).SingleOrDefault();
            if (parent == null) throw new ArgumentException(NOSINGLEPARENT_WITHCONTSTRAINTS, "source");
            return parent;
        }

        /// <summary>
        /// Returns a single parent chunk that matches the predicate given. Throws if more than one parent matches the type constraint and predicate, or if no parent was found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Chunk GetParent(this Chunk source, Predicate<Chunk> predicate)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var parent = ParentsOfChunk(source).Where(c => predicate.Invoke(c)).SingleOrDefault();
            if (parent == null) throw new ArgumentException(NOSINGLEPARENT_WITHCONTSTRAINTS, "source");
            return parent;
        }

        /// <summary>
        /// Returns a parent chunk's section of type T. Throws if more than one parent matches the type constraint, or if no parent was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetParent<T>(this Section source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            var quad = VirtualSectionRegistry.ResolveKeyFromType<T>();
            var parent = ParentsOfChunk(source.Owner).Where(c => c.Quad == quad).SingleOrDefault();
            if (parent == null || !(parent.Section is T)) throw new ArgumentException(NOSINGLEPARENT_WITHCONTSTRAINTS, "source");
            return (T)parent.Section;
        }

        /// <summary>
        /// Returns a parent chunk's section of type T. Throws if more than one parent matches the type constraint, or if no parent was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetParent<T>(this Chunk source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var quad = VirtualSectionRegistry.ResolveKeyFromType<T>();
            var parent = ParentsOfChunk(source).Where(c => c.Quad == quad).SingleOrDefault();
            if (parent == null || !(parent.Section is T)) throw new ArgumentException(NOSINGLEPARENT_WITHCONTSTRAINTS, "source");
            return (T)parent.Section;
        }

        /// <summary>
        /// Returns all parent chunks of <paramref name="source"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetParents(this Chunk source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            return ParentsOfChunk(source);
        }

        /// <summary>
        /// Returns all parent chunks' sections that are of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetParents<T>(this Chunk source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            return ParentsOfChunk(source).Select(c => c.Section).OfType<T>();
        }

        /// <summary>
        /// Returns all parent chunks of <paramref name="source"/>'s owner.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetParents(this Section source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            return ParentsOfChunk(source.Owner);
        }

        /// <summary>
        /// Returns the section of all parent chunks of <paramref name="source"/>'s owner that are of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetParents<T>(this Section source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            return ParentsOfChunk(source.Owner).Select(c => c.Section).OfType<T>();
        }

        /// <summary>
        /// Returns a single child chunk. Throws if more than one child exists, or if no child was found.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Chunk GetChild(this Chunk source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var child = ChildrenOfChunk(source).SingleOrDefault();
            if (child == null) throw new ArgumentException(NOSINGLECHILD, "source");
            return child;
        }

        /// <summary>
        /// Returns a single child chunk with Quad = <paramref name="quad"/>. Throws if more than one child matches the type constraint, or if no child was found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="quad"></param>
        /// <returns></returns>
        public static Chunk GetChild(this Chunk source, string quad)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var child = ChildrenOfChunk(source).Where(c => c.Quad == quad).SingleOrDefault();
            if (child == null) throw new ArgumentException(NOSINGLECHILD_WITHCONTSTRAINTS, "source");
            return child;
        }

        /// <summary>
        /// Returns a single child chunk that matches the predicate given. Throws if more than one child matches the type constraint and predicate, or if no child was found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Chunk GetChild(this Chunk source, Predicate<Chunk> predicate)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var child = ChildrenOfChunk(source).Where(c => predicate.Invoke(c)).SingleOrDefault();
            if (child == null) throw new ArgumentException(NOSINGLECHILD_WITHCONTSTRAINTS, "source");
            return child;
        }

        /// <summary>
        /// Returns a child chunk's section of type T. Throws if more than one child matches the type constraint, or if no child was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetChild<T>(this Section source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            var quad = VirtualSectionRegistry.ResolveKeyFromType<T>();
            var child = ChildrenOfChunk(source.Owner).Where(c => c.Quad == quad).SingleOrDefault();
            if (child == null || !(child.Section is T)) throw new ArgumentException(NOSINGLECHILD_WITHCONTSTRAINTS, "source");
            return (T)child.Section;
        }

        /// <summary>
        /// Returns a child chunk's section of type T. Throws if more than one child matches the type constraint, or if no child was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetChild<T>(this Chunk source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            var quad = VirtualSectionRegistry.ResolveKeyFromType<T>();
            var child = ChildrenOfChunk(source).Where(c => c.Quad == quad).SingleOrDefault();
            if (child == null || !(child.Section is T)) throw new ArgumentException(NOSINGLECHILD_WITHCONTSTRAINTS, "source");
            return (T)child.Section;
        }

        /// <summary>
        /// Returns all child chunks of <paramref name="source"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetChildren(this Chunk source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            return ChildrenOfChunk(source);
        }

        /// <summary>
        /// Returns all child chunks' sections that are of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetChildren<T>(this Chunk source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.container);
            return ChildrenOfChunk(source).Select(c => c.Section).OfType<T>();
        }

        /// <summary>
        /// Returns all child chunks of <paramref name="source"/>'s owner.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetChildren(this Section source)
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            return ChildrenOfChunk(source.Owner);
        }

        /// <summary>
        /// Returns the section of all child chunks of <paramref name="source"/>'s owner that are of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetChildren<T>(this Section source) where T : VirtualSection
        {
            EnsureSourceNotNull(source);
            EnsureContainerNotNull(source.owner);
            return ChildrenOfChunk(source.Owner).Select(c => c.Section).OfType<T>();
        }

        private static IEnumerable<Chunk> ParentsOfChunk(Chunk source)
        {
            return source.ReferencedBy.Select(r => r.Resolve()).Distinct();
        }

        private static IEnumerable<Chunk> ChildrenOfChunk(Chunk source)
        {
            return source.References.Select(r => r.Resolve()).Distinct();
        }

        private static void EnsureSourceNotNull(object source)
        {
            if (source == null) throw new ArgumentNullException("source");
        }

        private static void EnsureContainerNotNull(object container)
        {
            if (container == null) throw new ArgumentException("source is not inside a container", "source");
        }
    }
}
