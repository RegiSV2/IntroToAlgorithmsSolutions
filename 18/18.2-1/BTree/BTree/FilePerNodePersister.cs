using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BTree
{
    internal class FilePerNodePersister<TKey, TData> : IBTreePersister<TKey, TData>
        where TKey : IComparable<TKey>
    {
        private readonly string _metadataFileName;
        private readonly string _folder;
        private readonly IFormatter _nodeFormatter = new BinaryFormatter();

        public FilePerNodePersister(string folder)
        {
            if(folder == null)
                throw new NullReferenceException();
            _folder = folder;
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
            _metadataFileName = Path.Combine(_folder, "meta.info");
        }

        public void Save(Node<TKey, TData> node)
        {
            var fileName = NodeFileName(node.Id);
            using (var stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                _nodeFormatter.Serialize(stream, node);
        }

        public void SaveRoot(Node<TKey, TData> root)
        {
            using (var stream = File.Open(_metadataFileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
                writer.WriteLine(root.Id);
            Save(root);
        }

        public void Remove(Guid nodeId)
        {
            if(File.Exists(NodeFileName(nodeId)))
                File.Delete(NodeFileName(nodeId));
        }

        public Node<TKey, TData> Load(Guid nodeId)
        {
            var fileName = NodeFileName(nodeId);
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                return (Node<TKey, TData>)_nodeFormatter.Deserialize(stream);
        }

        private string NodeFileName(Guid nodeId)
        {
            return Path.Combine(_folder, nodeId.ToString());
        }
    }
}