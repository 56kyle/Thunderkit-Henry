using EntityStates;
using ModdedSurvivorCamel.Modules.Components;

namespace ModdedSurvivorCamel.SkillStates
{
    public class BaseModdedSurvivorCamelSkillState : BaseSkillState
    {
        protected ModdedSurvivorCamelController ModdedSurvivorLowerController;

        public override void OnEnter()
        {
            this.ModdedSurvivorLowerController = base.GetComponent<ModdedSurvivorCamelController>();
            base.OnEnter();
        }
    }
}