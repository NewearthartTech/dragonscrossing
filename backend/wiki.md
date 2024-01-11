# Add Non-Combat Encounter
1. Add the {Zone}State type to the BsonKnownTypes in base class NoncombatEncounterState file.
2. You would need to add a new {Zone}State.cs.
3. Add the {Zone}.cs that contains a method to create the non combat story for the zone.
4. Add the combat tile to the switch statement under method createNonCombatEncounter().
5. Add values to MapTileEntry, MapTileExit, MapLoreEncounterFinish, MapLocationEncounterFinish.
6. Add lore answers to lore_answer.json. Key is the question slug value and Value is the answer for that question.
