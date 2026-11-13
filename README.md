# Recoil Arena - Unity exam

Unity version: 6000.4.9f1. Open this folder from Unity Hub.
Scene: Assets/_EXAM/Scenes/Gameplay.unity.
Controls: click START; Space or left mouse button to shoot.

Implemented:
- Custom Inconsolata TMP font on buttons and SHOTS.
- Custom button Sprite with 16 px borders and Sliced Image components.
- Room AudioReverbFilter on DeathParticles, leaving source audio unchanged.
- Custom product name Recoil Arena and custom application icon.
- Canvas GraphicRaycaster for START.
- Rifle BoxCollider2D matching its sprite dimensions.
- Bullet collision impulse at the hit point.
- ImpactAudio on player and enemy rigidbodies, with volume proportional to contact impulse / fixed time step. Uses supplied hit.ogg.
- Final Boss B: ShotGun inherits Gun and fires five pellets across a 30 degree spread. Wave 6 is assigned in GameManager.

Validation: project compiled and START entered gameplay in the Editor. A missing input reference found during the smoke test was repaired; no full six-wave playthrough or successful Windows build is claimed.

The project contains no editor authoring helper. No-code items use serialized Unity components and assets. Library, Temp, Logs, UserSettings and build output are excluded from this archive.

Bonus bug candidate in original Gun.Shoot: bulletImpulseForce was ignored in favour of the literal 10. The field is now used. Bonus credit is decided by the instructor.

Audio attribution: Assets/_EXAM/Audio/Audio/sources.txt and Music/sources.txt. Inconsolata by Raph Levien, SIL Open Font License.
