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

        // Takes the index of units in July or February intake.
        // Arrays will everntually be dynamically created based on the 
        // length of the course and what semester the intake is in. These
        // numbers are based on a three year course for a February intake
        List<int> febList = new List<int> ();
        List<int> julyList = new List<int> ();

        // course class representing the units in the current course that the algorithm will sort.
        Course course;
        public StudyPlanAlgorithm(List<CoursePlan> sessionList, int numUnits, bool midYearIntake)
        {
            // Create new course.
            course = new Course(sessionList, numUnits, midYearIntake);
            // Check course created successfully. XYZ TO DO. for testing purposes.
            course.check();
        }
        public List<CoursePlan> RunAlgorithm(List<CoursePlan> sessionList)
        {
            try
            {
                // Set semester indexes for feb start, then check if mid year start and
                // switch sem 1/2 indexes if so.
                setSemesterIndex();
                CheckMidYearIntake();
                // Remove exemptions.
                RemoveExemptions(course.courseStructure);
                // Check and correct course structure.
                CheckCourseIsValid(course.courseStructure);
            }
            catch (ArgumentOutOfRangeException outOfRange)
            {
                System.Diagnostics.Debug.WriteLine(outOfRange.Message);
            }
            catch (IndexOutOfRangeException  outOfRange)
            {
                System.Diagnostics.Debug.WriteLine(outOfRange.Message);
            }
            // Update Session variable
            return course.ToList(course.courseStructure);
        }
        // This method sets the index positions of the semester arrays
        // based on the length of the course
        public void setSemesterIndex()
        {
            int unitCount = course.CourseDuration;
            int count = 0;

            // Setting Feb indexes
            for (int i = 0; i < unitCount; i++)
            {
                febList.Add(i);
                count++;
                if (count == 4)
                {
                    count = 0;
                    i += 4;
                }
            }
            // Setting July indexes
            for (int i = 4; i < unitCount; i++)
            {
                julyList.Add(i);
                count++;
                if (count == 4)
                {
                    count = 0;
                    i += 4;
                }
            }
        }
        // Checks if mid year start and swaps sem indexes if so.
        public void CheckMidYearIntake()
        {
            System.Diagnostics.Debug.WriteLine("In Check mid year intake");
            if (course.midYearIntake == true)
            {
                System.Diagnostics.Debug.WriteLine("In mid year intake = true");
                ChangeToMidYearIntake();
                System.Diagnostics.Debug.WriteLine("Changed to mid year intake");
            }
        }
        // This method swaps the July and Feb indexes for the algorithm
        // to change courses
        public void ChangeToMidYearIntake()
        {
            List<int> tempList = febList;
            febList = julyList;
            julyList = tempList;
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
                        // If the node to be added after does not have a prerequisite, 
                        // it can be safely moved
                        if (course.courseStructure.ElementAt(index).PreReq == null)
                        {
                            System.Diagnostics.Debug.WriteLine(node.Value.UnitName + " is in " + GetSemester(getLinkedListIndex(node.Value)) + ", it should be in " + node.Value.Semester + ". Rearranging...");
                            course.courseStructure.AddAfter(course.courseStructure.Find(course.courseStructure.ElementAt(index)), node.Value);
                            course.courseStructure.Remove(node);
                            unitSwapSuccess = true;
                            break;
                        }
                        // If the current node is not a prerequisite of the node to be added
                        // after, it can safely be moved. Checks through all prerequisites that
                        // the unit has. If one of the units matches then it does not move the unit
                        else
                        {
                            if (preReqFound(course.courseStructure.ElementAt(index).PreReq, node) == false)
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
                            if (course.courseStructure.ElementAt(index).PreReq == null)
                            {

                                System.Diagnostics.Debug.WriteLine(node.Value.UnitName + " is in " + GetSemester(getLinkedListIndex(node.Value)) + ", it should be in " + node.Value.Semester + ". Rearranging...");
                                course.courseStructure.AddBefore(course.courseStructure.Find(course.courseStructure.ElementAt(index)), node.Value);
                                course.courseStructure.Remove(node);
                                unitSwapSuccess = true;
                                break;
                            }
                            // If the current node is not a prerequisite of the node to be added
                            // after, it can safely be moved. Checks through all prerequisites that
                            // the unit has. If one of the units matches then it does not move the unit
                            else
                            {
                                if (preReqFound(course.courseStructure.ElementAt(index).PreReq, node) == false)
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
            }
            // If both iterations failed, there are no places for the unit to be placed and
            // an inefficient outcome has been reached
            if (unitSwapSuccess == false)
            {
                System.Diagnostics.Debug.WriteLine("Inefficient outcome has been reached, " + node.Value.UnitName + " cannot be placed in any other semester as they are full, manual override required. Press any key to exit");
                // Need to exit out of the algorithm here and take them to a screen which
                // notifies the user of a non optimal solution
            }
        }
        // This method returns true if units are found in the incorrect
        // order based in the prerequisites
        bool PrerequisitesOutOfOrder(LinkedList<Unit> courseStruct)
        {
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
                    System.Diagnostics.Debug.WriteLine("node looking at is:");
                    System.Diagnostics.Debug.Write(node.Value.UnitName);
                    string preReq = node.Value.PreReq.First();

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
        // after the unit it should be before, it is moved to before the unit.
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
                    var preReq = getLatestPrereqInCourse(node.Value.PreReq);
                    // Saving the node into a new value and saving what prerequisite it has
                    var hasPrereqNode = node;

                    // Looping forward in the list to search if the prerequisite is
                    // found after the unit
                    while (node != null)
                    {
                        var nextNode2 = node.Next;

                        // Check if the prerequisite is found after after the unit that it should be before
                        if (node.Value.UnitCode == preReq)
                        {
                            var preReqNode = node;
                            Console.WriteLine(node.Value.UnitName + " is after " + hasPrereqNode.Value.UnitName + ", it should be before. Rearranging...");
                            while (node != null)
                            {
                                var nextNode3 = node.Next;
                                // This checks if its the last node in the list. If so check if the
                                // node before the node that has the prerequisite has the same semester.
                                // If not then iterate backwards through the list until a suitable match is found
                                if (node.Next == null)
                                {
                                    // If the node with the prerequisite matches the correct semester of the prequisite, then
                                    // move the prerequisite before the node
                                    if (node.Value.Semester == GetSemester(getLinkedListIndex(hasPrereqNode.Value)) || node.Value.Semester == "Any")
                                    {
                                        course.courseStructure.AddBefore(hasPrereqNode, node.Value);
                                        course.courseStructure.Remove(node);
                                        break;
                                    }
                                    // If the node that is the prerequisite matches the correct semester of the node that
                                    // has the prequisite, then move the node that has the prerequisite after the prerequisite
                                    else if (hasPrereqNode.Value.Semester == GetSemester(getLinkedListIndex(node.Value)) || node.Value.Semester == "Any")
                                    {
                                        course.courseStructure.AddBefore(hasPrereqNode, node.Value);
                                        course.courseStructure.Remove(node);
                                        break;
                                    }
                                    // If the above two fail, iterate backwards through the list until a valid position
                                    // is found
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
                                // If the node after the prerequisie, has the correct semester of the value
                                // that is to be switched to, proceed. If not move onto the next node
                                if (node.Next.Value.Semester == GetSemester(getLinkedListIndex(hasPrereqNode.Value)) || node.Next.Value.Semester == "Any")
                                {
                                    // If the node to be moved is not going to be moved behind a prerequisite, proceed
                                    // otherwise move onto the next node
                                    if (preReqFound(node.Next.Value.PreReq, node) == false)
                                    {
                                        swapUnitPositions(node.Next, hasPrereqNode);
                                        break;
                                    }
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
        // This method will search for the prereq that is lowest in the course structure
        string getLatestPrereqInCourse(List<string> prereqList)
        {
            var currentLatestPreqIndex = 0;
            var foundPrereqIndex = 0;

            var node = course.courseStructure.First;
            // Check through all the prereqs in the list to find
            // the lowest one in the course structure
            foreach (string preReq in prereqList)
            {
                while (node != null)
                {
                    var nextNode = node.Next;
                    if (node.Value.UnitCode == preReq)
                    {
                        foundPrereqIndex = getLinkedListIndex(node.Value);
                        if (foundPrereqIndex > currentLatestPreqIndex)
                        {
                            currentLatestPreqIndex = foundPrereqIndex;
                        }
                    }
                    node = nextNode;
                }
            }
            return course.courseStructure.ElementAt(currentLatestPreqIndex).UnitCode;
        }
        // This method will search backwards from the current node through the list
        // to see if the current node can move down the list withouth it conflicting
        // with any prerequisites
        // This method will search backwards from the current node through the list
        // to see if the current node can move down the list withouth it conflicting
        // with any prerequisites
        bool preReqFound(List<string> prereqList, LinkedListNode<Unit> currentNode)
        {
            bool preReqBad = false;

            if (prereqList != null)
            {
                foreach (string prereq in prereqList)
                {
                    var node = currentNode;
                    while (node != null)
                    {
                        if (prereq == node.Value.UnitCode)
                        {
                            preReqBad = true;
                            return preReqBad;
                        }
                        node = node.Previous;
                    }
                }
            }
            return preReqBad;
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
        public void swapUnitPositions(LinkedListNode<Unit> first, LinkedListNode<Unit> second)
        {
            Unit temp = first.Value;
            first.Value = second.Value;
            second.Value = temp;
        }
    }
}