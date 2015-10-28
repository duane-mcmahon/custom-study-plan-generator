using custom_study_plan_generator.MetaObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace custom_study_plan_generator.StudyPlanAlgorithm
{
    // Have class inherit from 'Page' so that session is available
    public class StudyPlanAlgorithm : Page
    {
        
        string input;

        // Takes the index of units in July or February intake.
        // Arrays will everntually be dynamically created based on the 
        // length of the course and what semester the intake is in. These
        // numbers are based on a three year course for a February intake
        List<int> febList = new List<int> { 0, 1, 2, 3, 8, 9, 10, 11, 16, 17, 18, 19 };
        List<int> julyList = new List<int> { 4, 5, 6, 7, 12, 13, 14, 15, 20, 21, 22, 23 };

        // course class representing the units in the current course that the algorithm will sort.
        Course course;
        public List<CoursePlan> RunAlgorithm(List<CoursePlan> sessionList)
        {
            course = new Course(sessionList);
            
            try
            {
                // Displays the course structure in the console, prompts
                // user to remove exemptions and then checks if the course
                // is valid
                RemoveExemptions(course.courseStructure);
                CheckCourseIsValid(course.courseStructure);
            }
            catch (ArgumentOutOfRangeException outOfRange)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", outOfRange.Message);
            }
            catch (IndexOutOfRangeException  outOfRange)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", outOfRange.Message);
            }
            // Update Session variable
            return course.ToList(course.courseStructure);
        }
        // This method handles the removing of exemptions. Exemptions are 
        // passed from the exemptions page via the session variable studentPlan.
        void RemoveExemptions(LinkedList<Unit> courseStruct)
        {
            LinkedListNode<Unit> node;
            // Set up a list of removed exemptions (required for Modify page)
            List<string> removedExemptions = new List<string>();

            // Iterating through the course structure list and removing units
            // if their exemption status is true
            node = courseStruct.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value.Exempt == true)
                {
                    System.Diagnostics.Debug.WriteLine("Removing unit: " + node.Value.UnitName.ToString());
                    // Add the exemption unit name to the removed exemptions list
                    removedExemptions.Add(node.Value.UnitName);
                    course.courseStructure.Remove(node);
                    updateSemesterListSize();
                }
                node = nextNode;
            }

            // Save the removed exemptions to a session variable */
            Session["RemovedExemptions"] = removedExemptions;
        }
        // Updates the july and feb lists when a unit is removed
        void updateSemesterListSize()
        {
            for (int i = 0; i < febList.Count; i++)
            {
                if (febList.ElementAt(i) == course.courseStructure.Count())
                {
                    febList.RemoveAt(febList.Count - 1);
                    break;
                }
            }
            for (int i = 0; i < julyList.Count; i++)
            {
                if (julyList.ElementAt(i) == course.courseStructure.Count())
                {
                    julyList.RemoveAt(julyList.Count - 1);
                    break;
                }
            }
        }
        // Loop which constantly checks whether semester and prereqs are
        // in their correct spot
        void CheckCourseIsValid(LinkedList<Unit> courseStruct)
        {
            System.Diagnostics.Debug.WriteLine("\nOptimising... Please wait\n");

            int optimisationCount = 0;

            while (UnitsInWrongSemester(courseStruct) == true || PrerequisitesOutOfOrder(courseStruct) == true)
            {
                while (UnitsInWrongSemester(courseStruct) == true)
                {
                    CalculateSemesterAvailability();
                }
                while (PrerequisitesOutOfOrder(courseStruct) == true)
                {
                    CalculatePreRequisites();
                }
                // This is in the wrong spot, will work on a better breaker
                // if the algorithm starts looping
                optimisationCount++;
                System.Diagnostics.Debug.WriteLine("Optimisation count: " + optimisationCount);

                if (optimisationCount == 500)
                {
                    System.Diagnostics.Debug.WriteLine("\nOptimal course structure not found\n");
                    break;
                }
            }
        }
        // This method returns true if units are found in the incorrect
        // semesester
        bool UnitsInWrongSemester(LinkedList<Unit> courseStruct)
        {
            System.Diagnostics.Debug.WriteLine("\n UnitsInWrongSemester\n");
            bool unitsInWrongSemester = false;

            foreach (Unit unit in courseStruct)
            {
                if (unit.Semester != GetSemester(getLinkedListIndex(unit)) && unit.Semester != "Any")
                {
                    unitsInWrongSemester = true;
                    break;
                }
            }
            return unitsInWrongSemester;
        }
        // This method checks whether units are in the wrong semester. If
        // so then move them
        void CalculateSemesterAvailability()
        {
            System.Diagnostics.Debug.WriteLine("\nCalculateSemesterAvailability\n");
            var node = course.courseStructure.First;
            while (node != null)
            {
                var nextNode = node.Next;
                // Checks if the current unit node is sitting in the right semester
                if (node.Value.Semester != GetSemester(getLinkedListIndex(node.Value)) && node.Value.Semester != "Any")
                {
                    // Checks what the unit nodes semester is
                    if (node.Value.Semester == "July")
                    {
                        SwapUnitSemester(node, "July", julyList);
                    }
                    else if (node.Value.Semester == "Feb")
                    {
                        SwapUnitSemester(node, "Feb", febList);
                    }
                }
                node = nextNode;
            }
        }
        // This method swaps the units into the correct semester by either moving them up or
        // down the list based on their position. It will try down the list first, if 
        // this is not possible, it will move them up the list
        void SwapUnitSemester(LinkedListNode<Unit> node, string semester, List<int> semesterIndex)
        {
            bool unitSwapSuccess = false;
            // Iterates forward through the semester index list to find the
            // next possible spot to move the unit
            foreach (int index in semesterIndex)
            {
                // The loop reaches an index in the list that is more than its current index.
                if (index > getLinkedListIndex(node.Value))
                {
                    // If the found node to be replaced equals the correct semester of where it is 
                    // being moved to, it can safely be swapped to another semester
                    if (course.courseStructure.ElementAt(index).Semester != semester)
                    {
                        // If the current node is not a prerequisite of the node to be added
                        // after, it can safely be moved
                        if (course.courseStructure.ElementAt(index).PreReq != node.Value.UnitCode)
                        {
                            System.Diagnostics.Debug.WriteLine(node.Value.UnitName + " is in " + GetSemester(getLinkedListIndex(node.Value)) + ", it should be in " + node.Value.Semester + ". Rearranging...");
                            course.courseStructure.AddAfter(course.courseStructure.Find(course.courseStructure.ElementAt(index)), node.Value);
                            course.courseStructure.Remove(node);
                            unitSwapSuccess = true;
                            break;
                        }
                    }
                }
            }
            // If the forward iteration failed, iterate backwards through
            // the list to find a spot
            if (unitSwapSuccess == false)
            {
                IEnumerable<int> enumerableSemester = semesterIndex;
                foreach (int index in enumerableSemester.Reverse())
                {
                    // The loop reaches an index in the list that is less than its current index.
                    if (index < getLinkedListIndex(node.Value))
                    {
                        // If the found node to be replaced equals the correct semester of where it is 
                        // being moved to, it can safely be swapped to another semester
                        if (course.courseStructure.ElementAt(index).Semester != semester)
                        {
                            // If the current node is not a prerequisite of the node to be added
                            // after, it can safely be moved
                            if (course.courseStructure.ElementAt(index).PreReq != node.Value.UnitCode)
                            {
                                System.Diagnostics.Debug.WriteLine(node.Value.UnitName + " is in " + GetSemester(getLinkedListIndex(node.Value)) + ", it should be in " + node.Value.Semester + ". Rearranging...");
                                course.courseStructure.AddBefore(course.courseStructure.Find(course.courseStructure.ElementAt(index)), node.Value);
                                course.courseStructure.Remove(node);
                                unitSwapSuccess = true;
                                break;
                            }
                        }
                    }
                }
            }
            // If both iterations failed, there are no places for the unit to be placed and
            // an inefficient outcome has been reached
            if (unitSwapSuccess == false)
            {
                System.Diagnostics.Debug.WriteLine("Inefficient outcome has been reached, " + node.Value.UnitName + " cannot be placed in any other semester as they are full, manual override required.");
                // Need to exit out of the algorithm here and take them to a screen which
                // notifies the user of a non optimal solution
            }
        }
        // This method returns true if units are found in the incorrect
        // order based in the prerequisites
        bool PrerequisitesOutOfOrder(LinkedList<Unit> courseStruct)
        {
            System.Diagnostics.Debug.WriteLine("\nPrerequisitesOutOfOrder\n");
            bool prerequisitesOutOfOrder = false;
            var node = courseStruct.First;

            while (node != null)
            {
                var nextNode = node.Next;
                // Found a node that has a prerequisite
                if (node.Value.PreReq != null)
                {
                    // Saving the node into a new value and saving what prerequisite it has
                    var hasPrereqNode = node;
                    string preReq = node.Value.PreReq;

                    // Looping forward in the list
                    while (node != null)
                    {
                        // Check if the prerequisite is found after the node which has a prerequisite
                        if (node.Value.UnitCode == preReq)
                        {
                            prerequisitesOutOfOrder = true;
                            break;
                        }
                        node = node.Next;
                    }
                }
                node = nextNode;
            }
            return prerequisitesOutOfOrder;
        }
        // This method calculates the prerequisites. If prereq is found to be 
        // after the unit it should be before, it is moved to before the unit
        // Multiple pre-requisites have not been handled yet
        void CalculatePreRequisites()
        {
            System.Diagnostics.Debug.WriteLine("\n CalculatePreRequisites\n");
            var node = course.courseStructure.First;
            while (node != null)
            {
                var nextNode = node.Next;
                var previousNode = node.Previous;

                // Found a node that has a prerequisite
                if (node.Value.PreReq != null)
                {
                    // Saving the node into a new value and saving what prerequisite it has
                    var hasPrereqNode = node;
                    string preReq = node.Value.PreReq;

                    // Looping forward in the list to search if the prerequisite is
                    // found after the unit
                    while (node != null)
                    {
                        var nextNode2 = node.Next;

                        // Check if the prerequisite is found after after the unit that it should be before
                        if (node.Value.UnitCode == preReq)
                        {
                            var preReqNode = node;
                            System.Diagnostics.Debug.WriteLine(node.Value.UnitName + " is after " + hasPrereqNode.Value.UnitName + ", it should be before. Rearranging...");
                            while (node != null)
                            {
                                var nextNode3 = node.Next;
                                // This checks if its the last node in the list. If so check if the
                                // node before the node that has the prerequisite has the same semester.
                                // If not then iterate backwards through the list until a suitable match is found

                                // Non optimal solution still, some bugs exist
                                if (node.Next == null)
                                {
                                    if (node.Value.Semester == GetSemester(getLinkedListIndex(hasPrereqNode.Value)) || node.Value.Semester == "Any")
                                    {
                                        course.courseStructure.AddBefore(hasPrereqNode, node.Value);
                                        course.courseStructure.Remove(node);
                                        break;
                                    }
                                    else
                                    {
                                        var semesterNode = node;
                                        node = hasPrereqNode;
                                        while (node != null)
                                        {
                                            if (semesterNode.Value.Semester == GetSemester(getLinkedListIndex(node.Value)) || semesterNode.Value.Semester == "Any")
                                            {
                                                course.courseStructure.AddBefore(node, semesterNode.Value);
                                                course.courseStructure.Remove(semesterNode);
                                                break;
                                            }
                                            node = node.Previous;
                                        }
                                        break;
                                    }
                                }
                                if (node.Next.Value.Semester == GetSemester(getLinkedListIndex(hasPrereqNode.Value)) || node.Next.Value.Semester == "Any")
                                {
                                    course.courseStructure.AddBefore(hasPrereqNode, node.Next.Value);
                                    course.courseStructure.Remove(node.Next);
                                    course.courseStructure.AddAfter(preReqNode, hasPrereqNode.Value);
                                    course.courseStructure.Remove(hasPrereqNode);
                                    break;
                                }
                                node = nextNode3;
                            }
                        }
                        node = nextNode2;
                    }
                }
                node = nextNode;
            }
        }
        // Helper methoed to get the semester based on an index
        public string GetSemester(int x)
        {
            string semester = "Any";
            foreach (int index in febList)
            {
                if (x == index)
                {
                    semester = "Feb";
                    return semester;
                }
            }
            foreach (int index in julyList)
            {
                if (x == index)
                {
                    semester = "July";
                    return semester;
                }
            }
            return semester;
        }
        // Helper method to get the index of a unit
        public int getLinkedListIndex(Unit unit)
        {
            int index = course.courseStructure.Select((item, inx) => new { item, inx }).First(x => x.item == unit).inx;
            return index;
        }
    }
}