/* Copyright (C) 2014  Tomas Pluskal <plusik@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Formula
{
    /// <summary>
    /// This class generates molecular formulas within given mass range and elemental
    /// composition. There is no guaranteed order in which the formulas are
    /// generated.
    /// </summary>
    /// <example>
    /// Usage:
    /// <code>
    /// IsotopeFactory ifac = Isotopes.Instance;
    /// IIsotope c = ifac.GetMajorIsotope(&quot;C&quot;);
    /// IIsotope h = ifac.GetMajorIsotope(&quot;H&quot;);
    /// IIsotope n = ifac.GetMajorIsotope(&quot;N&quot;);
    /// IIsotope o = ifac.GetMajorIsotope(&quot;O&quot;);
    /// IIsotope p = ifac.GetMajorIsotope(&quot;P&quot;);
    /// IIsotope s = ifac.GetMajorIsotope(&quot;S&quot;);
    /// 
    /// MolecularFormulaRange mfRange = new MolecularFormulaRange();
    /// mfRange.Add(c, 0, 50);
    /// mfRange.Add(h, 0, 100);
    /// mfRange.Add(o, 0, 50);
    /// mfRange.Add(n, 0, 50);
    /// mfRange.Add(p, 0, 10);
    /// mfRange.Add(s, 0, 10);
    /// 
    /// MolecularFormulaGenerator mfg = new MolecularFormulaGenerator(builder, minMass,
    ///       maxMass, mfRange);
    /// double minMass = 133.003;
    /// double maxMass = 133.005;
    /// IMolecularFormulaSet mfSet = mfg.GetAllFormulas();
    /// </code>
    /// </example>
    /// <remarks>
    /// The code was originally developed for a MZmine 2 framework module, published
    /// in Pluskal et al. {@cdk.cite Pluskal2012}.
    /// </remarks>
    // @cdk.module formula
    // @author Tomas Pluskal
    // @cdk.created 2014-12-28
    // @cdk.githash
    public class MolecularFormulaGenerator
    {
        private readonly IChemObjectBuilder builder;

        /// <summary>
        /// Mass range to search by this instance of MolecularFormulaGenerator
        /// </summary>
        private readonly double minMass, maxMass;

        /// <summary>
        /// Internal arrays of isotopes (elements) used for the formula generation,
        /// their minimal and maximal counts, and the current counts, which
        /// correspond to the latest formula candidate.
        /// 
        /// For example, let's assume we set isotopes=[C,H,N], minCounts=[0,0,0], and
        /// maxCounts=[3,3,3]. Then, currentCounts will be iterated in the following
        /// order: [0,0,0], [1,0,0], [2,0,0], [3,0,0], [0,1,0], [1,1,0], [2,1,0],
        /// [3,1,0], [0,2,0], [1,2,0], [2,2,0], etc.
        /// 
        /// The lastIncreasedPosition index indicates the last position in
        /// currentCounts that was increased by calling IncreaseCounter(position)
        /// </summary>
        private readonly IIsotope[] isotopes;
        private readonly int[] minCounts, maxCounts, currentCounts;
        private int lastIncreasedPosition = 0;

        /// <summary>
        /// A flag indicating that the formula generation is running. If the search
        /// is finished (the whole search space has been examined) or the search is
        /// canceled by calling the Cancel() method, this flag is set to false.
        /// </summary>
        /// <seealso cref="Cancel"/>
        private bool searchRunning = true;

        /// <summary>
        /// Initiate the MolecularFormulaGenerator.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="minMass">Lower boundary of the target mass range</param>
        /// <param name="maxMass">Upper boundary of the target mass range</param>
        /// <param name="mfRange">A range of elemental compositions defining the search space</param>
        /// <exception cref="ArgumentException">
        ///            In case some of the isotopes in mfRange has undefined exact
        ///            mass or in case illegal parameters are provided (e.g.,
        ///            negative mass values or empty MolecularFormulaRange)</exception>
        /// <seealso cref="MolecularFormulaRange"/>
        public MolecularFormulaGenerator(IChemObjectBuilder builder,
                double minMass, double maxMass,
                MolecularFormulaRange mfRange)
        {
            Trace.TraceInformation("Initiate MolecularFormulaGenerator, mass range ", minMass,
                    "-", maxMass);

            // Check parameter values
            if ((minMass < 0.0) || (maxMass < 0.0))
            {
                throw (new ArgumentOutOfRangeException(
                        "The minimum and maximum mass values must be >=0"));
            }

            if ((minMass > maxMass))
            {
                throw (new ArgumentException(
                        "Minimum mass must be <= maximum mass"));
            }

            if ((mfRange == null) || (mfRange.Count == 0))
            {
                throw (new ArgumentException(
                        "The MolecularFormulaRange parameter must be non-null and must contain at least one isotope"));
            }

            // Save the parameters
            this.builder = builder;
            this.minMass = minMass;
            this.maxMass = maxMass;

            // Sort the elements by mass in ascending order. That speeds up
            // the search.
            SortedSet<IIsotope> isotopesSet = new SortedSet<IIsotope>(
                    new IIsotopeSorterByMass());
            foreach (var isotope in mfRange.GetIsotopes())
            {
                // Check if exact mass of each isotope is set
                if (isotope.ExactMass == null)
                    throw new ArgumentException(
                            "The exact mass value of isotope " + isotope
                                    + " is not set");
                isotopesSet.Add(isotope);
            }
            isotopes = isotopesSet.ToArray();

            // Save the element counts from the provided MolecularFormulaRange
            minCounts = new int[isotopes.Length];
            maxCounts = new int[isotopes.Length];
            for (int i = 0; i < isotopes.Length; i++)
            {
                minCounts[i] = mfRange.GetIsotopeCountMin(isotopes[i]);
                maxCounts[i] = mfRange.GetIsotopeCountMax(isotopes[i]);

                // Update the maximum count according to the mass limit
                int maxCountAccordingToMass = (int)Math.Floor(maxMass
                        / isotopes[i].ExactMass.Value);
                if (maxCounts[i] > maxCountAccordingToMass)
                    maxCounts[i] = maxCountAccordingToMass;

            }

            // Set the current counters to minimal values, initially
            currentCounts = Arrays.CopyOf(minCounts, minCounts.Length);
        }

        /// <summary>
        /// Returns next generated formula or null in case no new formula was found
        /// (search is finished). There is no guaranteed order in which the formulas
        /// are generated.
        /// </summary>
        public IMolecularFormula GetNextFormula()
        {
            // Main cycle iterating through element counters
            mainCycle: while (searchRunning)
            {
                double currentMass = CalculateCurrentMass();

                // Heuristics: if we are over the mass, it is meaningless to add
                // more atoms, so let's jump directly to the maximum count at the
                // position we increased last time.
                if (currentMass > maxMass)
                {
                    // Keep a lock on the currentCounts, because
                    // GetFinishedPercentage() might be called from another
                    // thread
                    lock (currentCounts)
                    {
                        currentCounts[lastIncreasedPosition] = maxCounts[lastIncreasedPosition];
                        IncreaseCounter(lastIncreasedPosition);
                    }
                    goto mainCycle;
                }

                // If the current formula mass fits in the target mass range, let's
                // return it
                if ((currentMass >= minMass) && (currentMass <= maxMass))
                {
                    IMolecularFormula cdkFormula = GenerateFormulaObject();
                    IncreaseCounter(0);
                    return cdkFormula;
                }

                // Increase the counter
                IncreaseCounter(0);
            }

            // All combinations tested, return null
            return null;
        }

        /// <summary>
        /// Generates a <see cref="IMolecularFormulaSet"/> by repeatedly calling {@link
        /// GetNextFormula()} until all possible formulas are generated. There is no
        /// guaranteed order to the formulas in the resulting 
        /// <see cref="IMolecularFormulaSet"/>.
        /// </summary>
        /// <remarks>
        /// Note: if some formulas were already generated by calling {@link
        /// GetNextFormula()} on this MolecularFormulaGenerator instance, those
        /// formulas will not be included in the returned</remarks>
        /// <seealso cref="IMolecularFormulaSet"/>.
        /// <seealso cref="GetNextFormula"/>
        public IMolecularFormulaSet GetAllFormulas()
        {
            IMolecularFormulaSet result = builder.CreateMolecularFormulaSet();
            IMolecularFormula nextFormula;
            while ((nextFormula = GetNextFormula()) != null)
            {
                result.Add(nextFormula);
            }
            return result;
        }

        /// <summary>
        /// Increases the internal counter array <see cref="currentCounts"/>[] at given position.
        /// If the position has reached its maximum value, its value is reset to the
        /// minimum value and the next position is increased.
        /// </summary>
        /// <param name="position">Index to the currentCounts[] array that should be increased</param>
        private void IncreaseCounter(int position)
        {
            // This should never happen, but let's check, just in case
            if (position >= currentCounts.Length)
            {
                throw new ArgumentException(
                        "Cannot increase the currentCounts counter at position "
                                + position);
            }

            lastIncreasedPosition = position;

            // Keep a lock on the currentCounts, because
            // GetFinishedPercentage() might be called from another thread
            lock (currentCounts)
            {

                if (currentCounts[position] < maxCounts[position])
                {
                    currentCounts[position]++;
                }
                else
                {
                    // Reset the value at given position, and increase the next one.
                    if (position < isotopes.Length - 1)
                    {
                        currentCounts[position] = minCounts[position];
                        IncreaseCounter(position + 1);
                    }
                    else
                    {
                        // If we are already at the last position, that means we
                        // have covered the whole search space and we can stop the
                        // search.
                        searchRunning = false;

                        // Copy the maxCounts[] array to currentCounts[]. This
                        // ensures that GetFinishedPercentage() will return 1.0
                        Array.Copy(maxCounts, 0, currentCounts, 0,
                                maxCounts.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the exact mass of the currently evaluated formula. Basically,
        /// it multiplies the currentCounts[] array by the masses of the isotopes in
        /// the isotopes[] array.
        /// </summary>
        private double CalculateCurrentMass()
        {
            double mass = 0;
            for (int i = 0; i < isotopes.Length; i++)
            {
                mass += currentCounts[i] * isotopes[i].ExactMass.Value;
            }

            return mass;
        }

        /// <summary>
        /// Generates a MolecularFormula object that contains the isotopes in the
        /// isotopes[] array with counts in the currentCounts[] array. In other
        /// words, generates a proper CDK representation of the currently examined
        /// formula.
        /// </summary>
        private IMolecularFormula GenerateFormulaObject()
        {
            IMolecularFormula formulaObject = builder
                    .CreateMolecularFormula();
            for (int i = 0; i < isotopes.Length; i++)
            {
                if (currentCounts[i] == 0)
                    continue;
                formulaObject.Add(isotopes[i], currentCounts[i]);
            }
            return formulaObject;
        }

        /// <summary>
        /// Returns a value between 0.0 and 1.0 indicating what portion of the search
        /// space has been examined so far by this MolecularFormulaGenerator. Before
        /// the first call to {@link GetNextFormula()}, this method returns 0. After
        /// all possible formulas are generated, this method returns 1.0 (the exact
        /// returned value might be slightly off due to rounding errors). This method
        /// can be called from any thread.
        /// </summary>
        public double GetFinishedPercentage()
        {
            double result = 0.0;
            double remainingPerc = 1.0;

            // Keep a lock on currentCounts, otherwise it might change during the
            // calculation
            lock (currentCounts)
            {
                for (int i = currentCounts.Length - 1; i >= 0; i--)
                {
                    double max = maxCounts[i];
                    if (i > 0)
                        max += 1.0;
                    result += remainingPerc * ((double)currentCounts[i] / max);
                    remainingPerc /= max;
                }
            }
            return result;
        }

        /// <summary>
        /// Cancel the current search. This method can be called from any thread. If
        /// another thread is executing the {@link GetNextFormula()} method, that
        /// method call will return immediately with null return value. If another
        /// thread is executing the {@link GetAllFormulas()} method, that method call
        /// will return immediately, returning all formulas generated until this
        /// moment. The search cannot be restarted once canceled - any subsequent
        /// calls to {@link GetNextFormula()} will return null.
        /// </summary>
        public void Cancel()
        {
            Trace.TraceInformation("Canceling MolecularFormulaGenerator");
            this.searchRunning = false;
        }

        /// <summary>
        /// A simple <see cref="IComparer{T}"/> implementation for sorting IIsotopes by their mass
        /// </summary>
        private class IIsotopeSorterByMass : IComparer<IIsotope>
        {
            public int Compare(IIsotope i1, IIsotope i2)
            {
                return i1.ExactMass.Value.CompareTo(i2.ExactMass.Value);
            }
        }
    }
}
