using UnityEngine;
using Zenject;

public class MainMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Container.BindInterfacesTo<ResourceManager>().AsSingle();
        Container.Bind<IKeyBindingsController>()
        .To<KeyBindingsController>()
        .AsSingle();
    }
}