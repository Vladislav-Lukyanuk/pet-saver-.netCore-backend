namespace animalFinder.Service.Interface
{
    public interface IViewRenderService
    {
        public string Render<TModel>(string name, TModel model);
    }
}
