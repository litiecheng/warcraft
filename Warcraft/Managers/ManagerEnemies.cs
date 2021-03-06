﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Warcraft.Units;
using Warcraft.Units.Orcs;
using Warcraft.Util;
using Warcraft.Buildings;
using Warcraft.Buildings.Neutral;
using Warcraft.Units.Humans;
using Warcraft.Commands;
using Microsoft.Xna.Framework;

namespace Warcraft.Managers
{
    class ManagerEnemies
    {
        ManagerMouse managerMouse;
        ManagerMap managerMap;
        public ManagerBuildings managerBuildings;
        public ManagerUnits managerUnits;

        Random random = new Random();

        public int index = 0;

        List<EA.ActionType> actionsTypes = new List<EA.ActionType>();
        List<int> actions = new List<int>();

        int nivel = 0;

        int[] times = { 200, 150, 100 };

		public ManagerEnemies(ManagerMouse managerMouse, ManagerMap managerMap, int index)
        {
            if (ManagerResources.BOT_FOOD.Count == 0)
            {
                ManagerResources.BOT_GOLD.Add(5000);
                ManagerResources.BOT_WOOD.Add(99999);
                ManagerResources.BOT_FOOD.Add(5);
                ManagerResources.BOT_OIL.Add(99999);
            }
            else
            {
				ManagerResources.BOT_GOLD[0] = 5000;
				ManagerResources.BOT_FOOD[0] = 5;
			}

			this.index = index;
            this.managerMap = managerMap;
            this.managerMouse = managerMouse;
			this.managerBuildings = new ManagerBotsBuildings(managerMouse, managerMap, index);
            this.managerUnits = new ManagerBotsUnits(managerMouse, managerMap, managerBuildings, index);

            actionsTypes.Add(EA.ActionType.BUILDING);
            actions.Add(0);

            actionsTypes.Add(EA.ActionType.TOWN_HALL);
			actions.Add(0);

			actionsTypes.Add(EA.ActionType.TOWN_HALL);
			actions.Add(0);

			actionsTypes.Add(EA.ActionType.TOWN_HALL);
			actions.Add(0);

            actionsTypes.Add(EA.ActionType.BUILDING);
			actions.Add(2);

			actionsTypes.Add(EA.ActionType.BUILDING);
			actions.Add(2);

            actionsTypes.Add(EA.ActionType.BUILDING);
			actions.Add(1);

			if (Warcraft.ISLAND > 0)
			{
				actionsTypes.Add(EA.ActionType.TOWN_HALL);
				actions.Add(0);

				actionsTypes.Add(EA.ActionType.TOWN_HALL);
				actions.Add(0);
			}

            actionsTypes.Add(EA.ActionType.MINING);
			actions.Add(4);

			actionsTypes.Add(EA.ActionType.MINING);
			actions.Add(4);

            actionsTypes.Add(EA.ActionType.BARRACKS);
			actions.Add(0);

            actionsTypes.Add(EA.ActionType.BARRACKS);
			actions.Add(1);

            if (Warcraft.ISLAND > 0)
            {
                actionsTypes.Add(EA.ActionType.BUILDING);
                actions.Add(5);
            }
		}

        public void LoadContent(ContentManager content)
        {
            managerBuildings.LoadContent(content);
            managerUnits.LoadContent(content);
        }

        public void Update()
		{
            managerBuildings.Update();
            managerUnits.Update();

            Builder builder = managerUnits.units.Find(u => u is Builder && u.workState == WorkigState.NOTHING) as Builder;
            Building greatHall = managerBuildings.buildings.Find(b => (b.information as InformationBuilding).Type == Util.Buildings.GREAT_HALL && b.isWorking);
			Barracks barrack = managerBuildings.buildings.Find(b => (b.information as InformationBuilding).Type == Util.Buildings.ORC_BARRACKS && b.isWorking) as Barracks;

            if (actionsTypes.Count > 0) 
            {
                switch (actionsTypes[0]) 
                {
                    case EA.ActionType.BUILDING:
                        if (builder != null)
                        {
                            Building building = (builder.commands[actions[0]] as BuilderBuildings).building;
							building.isPlaceSelected = true;

                            if (actionsTypes[0] == 0)
                                building.Position = Functions.CleanHalfPosition(managerMap, ManagerBuildings.goldMines[1].position);
                            else
                                building.Position = Functions.CleanHalfPosition(managerMap, greatHall.position);
                            
                            bool r = builder.commands[actions[0]].execute();

                            if (r)
                            {
                                actionsTypes.RemoveAt(0);
                                actions.RemoveAt(0);
                            }
                        }
                        break;
                    case EA.ActionType.MINING:
                        if (builder != null)
                        {
							bool r = builder.commands[actions[0]].execute();
                            if (r)
                            {
                                actionsTypes.RemoveAt(0);
                                actions.RemoveAt(0);
                            }
                        }
						break;
                    case EA.ActionType.BARRACKS:
                        if (barrack != null)
						{
                            bool r = barrack.commands[actions[0]].execute();
                            if (r)
                            {
                                actionsTypes.RemoveAt(0);
                                actions.RemoveAt(0);
                            }
						}
						break;
                    case EA.ActionType.TOWN_HALL:
                        if (greatHall != null)
						{
                            bool r = greatHall.commands[actions[0]].execute();
                            if (r)
                            {
                                actionsTypes.RemoveAt(0);
                                actions.RemoveAt(0);
                            }
						}
						break;
                }
            } 
            else
			{
                nivel++;

                if (nivel >= times[Warcraft.ISLAND] * Warcraft.FPS)
                {
                    nivel = 0;

                    if (ManagerResources.BOT_FOOD[0] == 0)
                    {
                        actionsTypes.Add(EA.ActionType.BUILDING);
                        actions.Add(2);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        actionsTypes.Add(EA.ActionType.BARRACKS);
                        if (random.NextDouble() > 0.5)
                        {
                            actions.Add(0);
                        }
                        else
                        {
                            actions.Add(1);
                        }
                    }
                }
            }
		}

        public void Draw(SpriteBatch spriteBatch)
        {
            managerBuildings.Draw(spriteBatch);
            managerUnits.Draw(spriteBatch);
		}

		public void DrawUI(SpriteBatch spriteBatch)
		{
			managerBuildings.DrawUI(spriteBatch);
			managerUnits.DrawUI(spriteBatch);
		}
    }
}
