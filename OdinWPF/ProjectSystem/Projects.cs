using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.ProjectSystem {
    public static class Projects {

        public static List<Project> projects = new List<Project>();
        public static void init() {

            load();
        }

        static void load() {
            StructFile file = new StructFile(App.userpath + "projects.txt", true);
            foreach (Line lp in file.coreLine.subs) {
                Project proj = new Project(lp.st);
                foreach (Line slp in lp.subs) {
                    if (slp.st.Equals("Modules")) {
                        foreach (Line ml in slp.subs) {
                            Step step = new Step(ml.st);
                            proj.addStep(step);
                            foreach (Line sml in ml.subs) {
                                if (sml.st.StartsWith("lore:")) step.lore = sml.value();
                                if (sml.st.StartsWith("hard")) step.difficult = true;
                                if (sml.st.StartsWith("done")) step.complete = true;
                            }
                        }
                    }
                }
                projects.Add(proj);
            }
        }
        public static void save() {
            StructFile file = new StructFile(App.userpath + "projects.txt", false);
            file.clear();
            foreach (Project p in projects) {
                Line pml = file.coreLine.add(p.name).add("Modules");
                foreach (Step s in p.steps) {
                    Line sl = pml.add(s.name);
                    if (s.lore != null) sl.add("lore:"+s.lore);
                    if (s.difficult) sl.add("hard");
                    if (s.complete) sl.add("done");
                }
            }
            file.save();
        }
        public static void openWindow() {
            ProjectsWindow win = new ProjectsWindow();
            win.Show();
        }
    }
}
