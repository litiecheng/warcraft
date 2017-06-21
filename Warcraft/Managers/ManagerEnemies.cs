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

        EA.PeasantController peasantController;
        EA.CityHallController cityHallController;
        EA.BarracksController barracksController;

		public ManagerEnemies(ManagerMouse managerMouse, ManagerMap managerMap, int index)
        {
            this.index = index;
            this.managerMap = managerMap;
            this.managerMouse = managerMouse;
			this.managerBuildings = new ManagerBotsBuildings(managerMouse, managerMap, index);
            this.managerUnits = new ManagerBotsUnits(managerMouse, managerMap, managerBuildings, index);

            peasantController = new EA.PeasantController(index, managerMap, managerBuildings, managerUnits);
            peasantController.SetTownHall(random.Next(0, 4));
            peasantController.SetBaracks(1000, 5, 0, 1);
            peasantController.SetFarms(500, 5, 1);
            peasantController.SetMiner(0, 1);

            cityHallController = new EA.CityHallController(500, 3, 0, index, managerMap, managerBuildings, managerUnits);

            barracksController = new EA.BarracksController(index, managerMap, managerBuildings, managerUnits);
            barracksController.SetArcher(500, 3, 0);
            barracksController.SetWarrior(500, 3, 0);
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
			
            if (builder != null)
            {
                if (greatHall == null)
                {
                    builder.commands[0].execute();

                    Building building = (builder.commands[0] as BuilderBuildings).building;
                    building.isPlaceSelected = true;
                    building.Position = peasantController.BuildTownHall(builder);
                }
                else
                {
                    if (peasantController.BuildBarracks()) 
                    {
                        builder.commands[1].execute();

						Building building = (builder.commands[1] as BuilderBuildings).building;
						building.isPlaceSelected = true;
                        building.Position = Functions.CleanPosition(managerMap, building.width, building.height);

                        peasantController.MultiplyBarracks(2);
					}
                    else if (peasantController.BuildFarms())
                    {
                        builder.commands[2].execute();

						Building building = (builder.commands[2] as BuilderBuildings).building;
						building.isPlaceSelected = true;
						building.Position = Functions.CleanPosition(managerMap, building.width, building.height);

                        peasantController.MultiplyFarms(2);
                    }
                    else if (peasantController.Miner())
                    {
                        builder.commands[4].execute();
                    }
                }
            }

            if (greatHall != null)
            {
                if (cityHallController.BuildPeon())
                {
					if (greatHall != null)
					{
						greatHall.commands[0].execute();
					}
				}
            }

            if (barrack != null)
            {
                if (barracksController.BuildWarrior())
                {
                    barrack.commands[0].execute();
                }

                if (barracksController.BuildArcher())
                {
                    barrack.commands[1].execute();
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
