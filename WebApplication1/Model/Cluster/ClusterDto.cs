namespace ClusterService
{
    public class ClusterDto
    {
        /// <summary>
        /// ClusterId классификатора
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории классификатора
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ClusterId родителя классификатора (о - является корневым)
        /// </summary>
        public int ParentId { get; set; }

    }
}
