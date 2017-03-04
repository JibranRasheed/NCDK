/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.IO.Listener;
using NCDK.IO.Setting;
using System.Collections.Generic;
using System.Linq;
using NCDK.Numerics;
using NCDK.IO.Formats;
using System;

namespace NCDK.IO
{
    /// <summary>
    /// Provides some basic functionality for readers and writers. This includes
    /// managing the <see cref="IChemObjectIOListener"/>'s and managing of <see cref="IOSetting"/>'s.
    /// The IOSettings are managed via the <see cref="SettingManager"/> class with most
    /// method's wrapped to more descriptive method names (e.g.
    /// {@link SettingManager#Get(string)} is invoked by {@link #GetSetting(string)}).
    ///
    // @author johnmay
    // @cdk.module io
    // @cdk.githash
    // @cdk.created 20.03.2012
    /// </summary>
    public abstract class ChemObjectIO : IChemObjectIO
    {
        /// <summary>
        /// Holder of reader event listeners.
        /// </summary>
        private List<IChemObjectIOListener> listeners = new List<IChemObjectIOListener>(2);
        private SettingManager<IOSetting>   settings  = new SettingManager<IOSetting>();
        public virtual ICollection<IChemObjectIOListener> Listeners => listeners;
        public virtual SettingManager<IOSetting> IOSettings => settings;
        public abstract IResourceFormat Format { get; }
        public S Add<S>(S setting) where S: IOSetting
        {
            return (S)settings.Add(setting);
        }

        public void AddSettings(IEnumerable<IOSetting> settings)
        {
            foreach (var setting in settings) {
                if (this.settings.Has(setting.Name))
                {
                    try
                    {
                        this.settings[setting.Name].Setting = setting.Setting;
                    }
                    catch (CDKException)
                    {
                        // setting value was invalid (ignore as we already have a value for this setting
                        // and we can't throw CDKException as IChemObject is in interfaces module)
                    }
                }
                else
                {
                    this.settings.Add(setting);
                }
            }
        }

        /// <summary>
        /// Fires <see cref="IChemObjectIOListener.ProcessIOSettingQuestion(IOSetting)"/> for all managed listeners.
        /// </summary>
        /// <param name="setting">the setting to process</param>
        protected void FireIOSettingQuestion(IOSetting setting) {
            foreach (var listener in listeners) {
                listener.ProcessIOSettingQuestion(setting);
            }
        }

        public abstract bool Accepts(Type type);
        public abstract void Close();
        public abstract void Dispose();
    }
}
